using CQRS.Application.DTO;
using MediatR;

namespace CQRS.Application.Features.BatchSerial.Queries.GetById
{
    public record GetBatchSerialByIdQuery(int Id) : IRequest<BatchSerialDto>
    {
    }
}
