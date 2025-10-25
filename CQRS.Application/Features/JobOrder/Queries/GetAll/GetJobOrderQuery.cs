using CQRS.Application.DTO;
using MediatR;

namespace CQRS.Application.Features.JobOrder.Queries.GetAll
{
    public record GetJobOrderQuery : IRequest<List<JobOrderDto>>
    {
    }
}
