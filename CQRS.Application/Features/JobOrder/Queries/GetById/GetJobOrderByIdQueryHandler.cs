using AutoMapper;
using CQRS.Application.Contracts.Interface;
using CQRS.Application.DTO;
using CQRS.Application.Shared.Exceptions;
using MediatR;

namespace CQRS.Application.Features.JobOrder.Queries.GetById
{
    public class GetJobOrderByIdQueryHandler(IMapper mapper, IJobOrderRepository jobOrderRepository) : IRequestHandler<GetJobOrderByIdQuery, JobOrderDto>
    {
        private readonly IMapper _mapper = mapper;
        private readonly IJobOrderRepository _jobOrderRepository = jobOrderRepository;

        public async Task<JobOrderDto> Handle(GetJobOrderByIdQuery request, CancellationToken cancellationToken)
        {
            // Query the database
            var joborder = await _jobOrderRepository.GetByIdAsync(request.Id) ?? throw new NotFoundException(nameof(JobOrder), request.Id);

            // Convert data objects to DTO
            var data = _mapper.Map<JobOrderDto>(joborder);

            // Return DTO object
            return data;
        }
    }
}
