using AutoMapper;
using CQRS.Application.Contracts.Interface;
using CQRS.Application.Contracts.Logging;
using CQRS.Application.DTO;
using MediatR;

namespace CQRS.Application.Features.JobOrder.Queries.GetAll
{
    public class GetJobOrderQueryHandler(IMapper mapper,
        IJobOrderRepository jobOrderRepository,
        IAppLogger<GetJobOrderQueryHandler> appLogger) :
        IRequestHandler<GetJobOrderQuery, List<JobOrderDto>>
    {
        private readonly IMapper _mapper = mapper;
        private readonly IJobOrderRepository _jobOrderRepository = jobOrderRepository;
        private readonly IAppLogger<GetJobOrderQueryHandler> _logger = appLogger;

        public async Task<List<JobOrderDto>> Handle(GetJobOrderQuery request, CancellationToken cancellationToken)
        {
            // Query the database
            var joborders = await _jobOrderRepository.GetAsync();

            // Convert data object to DTO
            var data = _mapper.Map<List<JobOrderDto>>(joborders);

            // Return the list of DTO object
            _logger.LogInformation("Product retrieve successfully.");

            return data;
        }
    }
}
