namespace CQRS.Application.Shared.Response
{
    public class CustomResultResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? Id { get; set; } = string.Empty;
        public IDictionary<string, string[]>? ValidationErrors { get; set; } // Get the list of errors
    }
}
