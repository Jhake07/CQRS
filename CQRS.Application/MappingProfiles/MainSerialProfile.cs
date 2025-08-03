using AutoMapper;
using CQRS.Application.Features.BatchSerial.Commands.CreateBatchSerial;
using CQRS.Domain;

namespace CQRS.Application.MappingProfiles
{
    public class MainSerialProfile : Profile
    {
        public MainSerialProfile()
        {
            CreateMap<CreateBatchSerialCommand, MainSerial>().ReverseMap();
        }
    }
}
