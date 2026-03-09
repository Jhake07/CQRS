namespace CQRS.Application.Shared.Response
{
    public class CustomResultResponse
    {
        public bool IsSuccess { get; init; }
        public string Message { get; init; } = string.Empty;
        public string? Id { get; init; }
        // Validation errors standardized: key = field, value = list of errors
        public IDictionary<string, string[]>? ValidationErrors { get; init; }
        // SUCCESS (no data)
        public static CustomResultResponse Success(string message, string? id = null) =>
            new()
            {
                IsSuccess = true,
                Message = message,
                Id = id
            };
        // FAILURE (validation or other business error)
        public static CustomResultResponse Failure(string message, IDictionary<string, string[]>? errors = null) =>
            new()
            {
                IsSuccess = false,
                Message = message,
                ValidationErrors = errors
            };
    }
    // GENERIC VERSION FOR RETURNING DATA
    public class CustomResultResponse<T> : CustomResultResponse
    {
        public T? Data { get; init; }
        // SUCCESS with data
        public static CustomResultResponse<T> Success(string message, T? data, string? id = null) =>
          new()
          {
              IsSuccess = true,
              Message = message,
              Data = data,
              Id = id
          };
        // FAILURE with validation errors
        public new static CustomResultResponse<T> Failure(string message, IDictionary<string, string[]>? errors = null) =>
            new()
            {
                IsSuccess = false,
                Message = message,
                ValidationErrors = errors
            };
    }
}