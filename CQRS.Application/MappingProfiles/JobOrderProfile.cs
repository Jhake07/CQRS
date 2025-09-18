using AutoMapper;
using CQRS.Application.Features.JobOrder.Commands.Create;
using CQRS.Domain;

namespace CQRS.Application.MappingProfiles
{
    public class JobOrderProfile : Profile
    {
        public JobOrderProfile()
        {
            CreateMap<CreateJobOrderCommand, JobOrder>();
        }
    }
}
