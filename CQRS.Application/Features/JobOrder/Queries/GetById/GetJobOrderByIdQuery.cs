using CQRS.Application.DTO;
using MediatR;

namespace CQRS.Application.Features.JobOrder.Queries.GetById
{
    public record GetJobOrderByIdQuery(int Id) : IRequest<JobOrderDto>
    {
    }
}
