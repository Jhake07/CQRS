using CQRS.Application.Shared.Response;
using MediatR;

namespace CQRS.Application.Features.JobOrder.Commands.Create
{
    public class CreateJobOrderCommand : IRequest<CustomResultResponse>
    {
        public required string JONo { get; set; }
        public required string BatchSerial_ContractNo { get; set; }
        public string Stats { get; set; } = string.Empty;
        public string OrderType { get; set; } = string.Empty;
        public int OrderQty { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int Line1Qty { get; set; }
        public int ProcessOrderQtyLine1 { get; set; }
        public int Line2Qty { get; set; }
        public int ProcessOrderQtyLine2 { get; set; }
        public required string ISNo { get; set; }
        public int LeftQty { get; set; }
    }
}
