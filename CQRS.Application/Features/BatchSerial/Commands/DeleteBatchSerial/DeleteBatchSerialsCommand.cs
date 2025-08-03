using CQRS.Application.Shared.Response;
using MediatR;

namespace CQRS.Application.Features.BatchSerial.Commands.DeleteBatchSerial
{
    public class DeleteBatchSerialsCommand : IRequest<CustomResultResponse>
    {
        public required int Id { get; set; }
        public string? Status { get; set; } = "Cancelled"; // Default status for deletion
    }
}
