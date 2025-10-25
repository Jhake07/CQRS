using AutoMapper;
using CQRS.Application.DTO;
using CQRS.Application.Features.JobOrder.Commands.Create;
using CQRS.Application.Features.JobOrder.Commands.Update;
using CQRS.Domain;

namespace CQRS.Application.MappingProfiles
{
    public class JobOrderProfile : Profile
    {
        public JobOrderProfile()
        {
            CreateMap<CreateJobOrderCommand, JobOrder>();
            CreateMap<UpdateJobOrderCommand, JobOrder>();
            CreateMap<JobOrderDto, JobOrder>().ReverseMap();
        }
    }
}
