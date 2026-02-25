using AutoMapper;
using CQRS.Application.Features.ScannedUnit.Commands.CreateScannedUnit;
using CQRS.Domain;

namespace CQRS.Application.MappingProfiles
{
    public class ScannedUnitProfile : Profile
    {
        public ScannedUnitProfile()
        {

            // Mapping from CreateScannedUnitCommand to ScannedUnit entity
            // This will map the properties of CreateScannedUnitCommand to the corresponding properties in the ScannedUnit entity
            CreateMap<CreateScannedUnitCommand, ScannedUnits>();
        }
    }
}
