using CQRS.Application.Shared.Response;
using MediatR;

namespace CQRS.Application.Features.BatchSerial.Commands.CreateBatchSerial
{
    public class CreateBatchSerialCommand : IRequest<CustomResultResponse>
    {
        public required string ContractNo { get; set; }
        public string Customer { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string DocNo { get; set; } = string.Empty;
        public int BatchQty { get; set; }
        public int OrderQty { get; set; }
        public int DeliverQty { get; set; }
        public int RemainingQty { get; set; }
        public string Status { get; set; } = string.Empty;
        public required string SerialPrefix { get; set; }
        public required string StartSNo { get; set; }
        public required string EndSNo { get; set; }
        public required string Item_ModelCode { get; set; }
        public required string CreatedBy { get; set; }
    }
}
