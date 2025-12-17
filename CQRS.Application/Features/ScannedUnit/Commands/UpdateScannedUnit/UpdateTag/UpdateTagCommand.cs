using CQRS.Application.Shared.Response;
using MediatR;

namespace CQRS.Application.Features.ScannedUnit.Commands.UpdateScannedUnit.UpdateTag
{
    public record UpdateTagCommand(string MainSerial, string NewTagNo) : IRequest<CustomResultResponse>
    {
    }
}
