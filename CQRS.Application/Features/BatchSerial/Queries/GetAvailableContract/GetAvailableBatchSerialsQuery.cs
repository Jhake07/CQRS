using CQRS.Application.DTO;
using MediatR;

namespace CQRS.Application.Features.BatchSerial.Queries.GetAvailableContract
{
    public record GetAvailableBatchSerialsQuery : IRequest<List<BatchSerialDto>>
    {
    }
}
