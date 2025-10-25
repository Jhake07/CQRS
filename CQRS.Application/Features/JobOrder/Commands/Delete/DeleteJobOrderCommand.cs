using CQRS.Application.Shared.Response;
using MediatR;

namespace CQRS.Application.Features.JobOrder.Commands.Delete
{
    public class DeleteJobOrderCommand : IRequest<CustomResultResponse>
    {
        public required int Id { get; set; }
        public string? Status { get; set; } = "Cancelled"; // Default status for deletion
    }
}
