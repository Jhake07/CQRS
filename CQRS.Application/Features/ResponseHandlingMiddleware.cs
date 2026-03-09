using CQRS.Application.Shared.Response;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
namespace CQRS.Application.Features
{
    public class ResponseHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        public ResponseHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            // Skip non-API or static requests
            if (ShouldSkipWrapping(context))
            {
                await _next(context);
                return;
            }
            var originalBody = context.Response.Body;
            using var memStream = new MemoryStream();
            context.Response.Body = memStream;
            await _next(context);
            // Only wrap success responses (2xx)
            if (context.Response.StatusCode >= 200 && context.Response.StatusCode < 300)
            {
                context.Response.Body.Seek(0, SeekOrigin.Begin);
                var rawBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
                if (ShouldWrapResponse(context, rawBody))
                {
                    var wrapped = WrapResponse(rawBody);
                    context.Response.Body = originalBody;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(wrapped);
                    return;
                }
            }
            // Not wrapping → copy body back
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            await memStream.CopyToAsync(originalBody);
            context.Response.Body = originalBody;
        }
        // -----------------------------
        // Helpers
        // -----------------------------
        private static bool ShouldSkipWrapping(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLowerInvariant() ?? "";
            return
                path.StartsWith("/swagger") ||
                path.StartsWith("/swagger-ui") ||
                path.StartsWith("/swaggerindex") ||
                path.EndsWith(".html") ||
                path.EndsWith(".css") ||
                path.EndsWith(".js") ||
                path.EndsWith(".png") ||
                path.EndsWith(".jpg") ||
                path.EndsWith(".ico") ||
                path.EndsWith(".map") ||
                path.Contains("swagger");
        }
        private static bool ShouldWrapResponse(HttpContext context, string body)
        {
            if (string.IsNullOrWhiteSpace(body))
                return true;
            var contentType = context.Response.ContentType?.ToLowerInvariant() ?? "";
            var trimmed = body.AsSpan().TrimStart();
            bool looksJson =
                contentType.Contains("application/json") ||
                (trimmed.Length > 0 && (trimmed[0] == '{' || trimmed[0] == '['));
            if (!looksJson)
                return false;
            // Avoid double wrapping
            try
            {
                using var doc = JsonDocument.Parse(body);
                if (doc.RootElement.TryGetProperty("isSuccess", out _))
                    return false;
            }
            catch
            {
                return false;
            }
            return true;
        }
        private static string WrapResponse(string body)
        {
            object? data = null;
            try
            {
                data = JsonSerializer.Deserialize<object>(body,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch
            {
                data = body;
            }
            var wrapper = CustomResultResponse<object>.Success("Success", data);
            return JsonSerializer.Serialize(wrapper);
        }
    }
}