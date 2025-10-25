using AutoMapper;
using CQRS.Application.Contracts.Interface;
using CQRS.Application.Contracts.Logging;
using CQRS.Application.DTO;
using MediatR;

namespace CQRS.Application.Features.BatchSerial.Queries.GetAvailableContract
{
    public class GetAvailableBatchSerialsQueryHandler(IMapper mapper, IBatchSerialRepository batchSerialRepository, IAppLogger<GetAvailableBatchSerialsQueryHandler> logger) :
       IRequestHandler<GetAvailableBatchSerialsQuery, List<BatchSerialDto>>
    {
        private readonly IMapper _mapper = mapper;
        private readonly IBatchSerialRepository _batchSerialRepository = batchSerialRepository;
        private readonly IAppLogger<GetAvailableBatchSerialsQueryHandler> _logger = logger;

        public async Task<List<BatchSerialDto>> Handle(GetAvailableBatchSerialsQuery request, CancellationToken cancellationToken)
        {
            // Query the database
            var batch = await _batchSerialRepository.GetAvailableBatchSerial();
            // Convert data object to DTO
            var data = _mapper.Map<List<BatchSerialDto>>(batch);

            // Return the list of DTO object
            _logger.LogInformation("Batch serial retrieve successfully.");
            return data;
        }
    }
}
