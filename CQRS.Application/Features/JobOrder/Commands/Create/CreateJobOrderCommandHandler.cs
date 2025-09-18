using AutoMapper;
using CQRS.Application.Contracts.Interface;
using CQRS.Application.Shared.Exceptions;
using CQRS.Application.Shared.Response;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CQRS.Application.Features.JobOrder.Commands.Create
{
    public class CreateJobOrderCommandHandler(IMapper mapper, IJobOrderRepository jobOrderRepository, ILogger<CreateJobOrderCommandHandler> logger)
        : IRequestHandler<CreateJobOrderCommand, CustomResultResponse>
    {
        private readonly IMapper _mapper = mapper;
        private readonly IJobOrderRepository _jobOrderRepository = jobOrderRepository;
        private readonly ILogger<CreateJobOrderCommandHandler> _logger = logger;

        public async Task<CustomResultResponse> Handle(CreateJobOrderCommand request, CancellationToken cancellationToken)
        {
            try
            {
                //validate data
                var validator = new CreateJobOrderCommandValidator(_jobOrderRepository);
                var validationResult = await validator.ValidateAsync(request, cancellationToken);

                if (validationResult.Errors.Count != 0)
                {
                    _logger.LogError("Validation errors occurred while creating job order.");
                    throw new BadRequestException("Validation Failed", validationResult);
                }

                // Map the command to the domain entity
                var jobOrder = _mapper.Map<Domain.JobOrder>(request);

                // Save Job Order
                await _jobOrderRepository.CreateAsync(jobOrder);
                _logger.LogInformation("Job Order created successfully with ID: {JobOrderId}", jobOrder.Id);

                // Return success response
                return new CustomResultResponse
                {
                    IsSuccess = true,
                    Message = "Job Order created successfully.",
                    Id = jobOrder.Id.ToString() // Ensure the ID is converted to string if necessary
                };
            }
            catch (BadRequestException ex)
            {
                _logger.LogError(ex, "Validation error occurred for JobOrderNo: {JobOrderNo}", request.JONo);
                // Return structured validation errors
                return new CustomResultResponse
                {
                    IsSuccess = false,
                    Message = "Handler Validation failed.",
                    ValidationErrors = ex.ValidationErrors, // Include validation errors here
                    Id = null
                };
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, "An error occurred while processing CreateBatchSerialCommand for ContractNo: {JobOrderNo}", request.JONo);

                // Return a generic error response
                return new CustomResultResponse
                {
                    IsSuccess = false,
                    Message = "An error occurred while processing your request.",
                    Id = null
                };
            }
        }
    }
}
