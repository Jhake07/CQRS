using CQRS.Application.DTO;
using MediatR;

namespace CQRS.Application.Features.BatchSerial.Queries.GetByContractNo
{
    public record GetBatchSerialByContractNoQuery(string ContractNo) : IRequest<BatchSerialDto>
    {
    }
}
