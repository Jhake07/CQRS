using AutoMapper;
using CQRS.Application.DTO;
using CQRS.Application.Features.BatchSerial.Commands.CreateBatchSerial;
using CQRS.Application.Features.BatchSerial.Commands.DeleteBatchSerial;
using CQRS.Application.Features.BatchSerial.Commands.UpdateBatchSerial;
using CQRS.Domain;

namespace CQRS.Application.MappingProfiles
{
    public class BatchSerialProfile : Profile
    {
        public BatchSerialProfile()
        {
            CreateMap<CreateBatchSerialCommand, BatchSerial>();
            CreateMap<UpdateBatchSerialCommand, BatchSerial>();
            CreateMap<DeleteBatchSerialsCommand, BatchSerial>();
            CreateMap<BatchSerialDto, BatchSerial>().ReverseMap();
        }
    }
}
