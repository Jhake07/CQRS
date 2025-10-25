using CQRS.Application.Shared.Response;
using MediatR;

namespace CQRS.Application.Features.JobOrder.Commands.Update
{
    public class UpdateJobOrderCommand : IRequest<CustomResultResponse>
    {
        public required int Id { get; set; }
        public required string BatchSerial_ContractNo { get; set; } // Used for validating order quantity
        public required string JoNo { get; set; } // Used for updating        
        public int OrderQty { get; set; }
        //public int NewOrderQty { get; set; } // Used for validating order quantity
        public int ProcessOrder { get; set; }
    }
}
