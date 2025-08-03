namespace CQRS.Application.Shared.Response
{
    public class IdentityResultResponse : CustomResultResponse
    {
        public string Email { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }
}
