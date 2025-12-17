using CQRS.Application.Shared.Response;
using MediatR;

namespace CQRS.Application.Features.ScannedUnit.Commands.UpdateScannedUnit.UpdateAccessories
{
    public record UpdateAccessoriesCommand(string mainSerial, string accessoriesSerial) : IRequest<CustomResultResponse>
    {
    }
}
