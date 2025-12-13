using CQRS.Application.Shared.Response;
using MediatR;

namespace CQRS.Application.Features.ScannedUnit.Commands.UpdateScannedUnit.UpdateComponent
{
    public record UpdateComponentsCommand(
     string MainSerial,
     string MotherboardSerial,
     string PcbiSerial,
     string PowerSupplySerial
 ) : IRequest<CustomResultResponse>;


}