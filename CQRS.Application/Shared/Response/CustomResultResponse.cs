namespace CQRS.Application.Shared.Response
{
    public class CustomResultResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? Id { get; set; } = string.Empty;
        public IDictionary<string, string[]>? ValidationErrors { get; set; } // Get the list of errors

        // Success 
        public static CustomResultResponse Success(string message, string? id = null) => new()
        {
            IsSuccess = true,
            Message = message,
            Id = id
        };

        // Failure 
        public static CustomResultResponse Failure(string message, IDictionary<string, string[]>? errors = null) => new()
        {
            IsSuccess = false,
            Message = message,
            ValidationErrors = errors,
            Id = null
        };

    }
}
