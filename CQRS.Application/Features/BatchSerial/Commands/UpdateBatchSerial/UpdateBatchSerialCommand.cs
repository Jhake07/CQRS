using CQRS.Application.Shared.Response;
using MediatR;

namespace CQRS.Application.Features.BatchSerial.Commands.UpdateBatchSerial
{
    public class UpdateBatchSerialCommand : IRequest<CustomResultResponse>
    {
        public required int Id { get; set; }
        public required string ContractNo { get; set; } // Used for updating
        public string? Customer { get; set; }
        public string? Address { get; set; }
        public string? DocNo { get; set; }
        public string? ItemModelCode { get; set; }
    }
}
