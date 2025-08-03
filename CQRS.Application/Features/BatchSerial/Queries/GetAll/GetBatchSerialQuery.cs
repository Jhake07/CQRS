using CQRS.Application.DTO;
using MediatR;

namespace CQRS.Application.Features.BatchSerial.Queries.GetAll
{
    public record GetBatchSerialQuery : IRequest<List<BatchSerialDto>>
    {
    }
}
