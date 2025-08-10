using Microsoft.AspNetCore.Mvc;

namespace CQRS.Application.Shared.Response
{
    public class ExtendedProblemDetails : ProblemDetails
    {
        public string? Id { get; set; }
        public IDictionary<string, string[]>? ValidationErrors { get; set; }
    }

}
