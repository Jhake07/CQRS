using CQRS.Application.Shared.Response;
using MediatR;

namespace CQRS.Application.Features.ScannedUnit.Commands.CreateScannedUnit
{
    public class CreateScannedUnitCommand : IRequest<CustomResultResponse>
    {
        public required string MainSerial { get; set; }
    }
}
