using CQRS.Application.Contracts.Interface;
using CQRS.Application.Shared.Exceptions;
using CQRS.Application.Shared.Response;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CQRS.Application.Features.JobOrder.Commands.Delete
{
    public class DeleteJobOrderCommandHandler(ILogger<DeleteJobOrderCommandHandler> logger, IJobOrderRepository jobOrderRepository)
        : IRequestHandler<DeleteJobOrderCommand, CustomResultResponse>
    {
        private readonly ILogger<DeleteJobOrderCommandHandler> _logger = logger;
        private readonly IJobOrderRepository _jobOrderRepository = jobOrderRepository;

        public async Task<CustomResultResponse> Handle(DeleteJobOrderCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check and get the details of Job Order
                var jobOrder = await _jobOrderRepository.GetJobOrderById(request.Id);
                if (jobOrder == null)
                {
                    _logger.LogWarning("Job Order details with Id {Id} not found.", request.Id);
                    return new CustomResultResponse
                    {
                        IsSuccess = false,
                        Message = $"Job Order with Id {request.Id} was not found."
                    };
                }

                // Only Job Order with status 'Open' can be cancelled
                if (!string.Equals(jobOrder.Stats, "Open", StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogInformation("Job Order {JoNo} is already in '{Status}' status. No action taken.", jobOrder.JoNo, jobOrder.Stats);
                    return new CustomResultResponse
                    {
                        IsSuccess = false,
                        Message = $"Job Order is already in '{jobOrder.Stats}' status.",
                        Id = jobOrder.JoNo.ToString()
                    };
                }

                // Explicitly set the cancellation status
                jobOrder.Stats = "Cancelled";

                // Update the job order status to Cancelled
                await _jobOrderRepository.UpdateAsync(jobOrder);
                _logger.LogInformation("Job Order successfully cancelled");

                return new CustomResultResponse
                {
                    IsSuccess = true,
                    Message = $"Job Order '{jobOrder.JoNo}' successfully cancelled",
                    Id = jobOrder.JoNo.ToString()
                };
            }
            catch (BadRequestException ex)
            {
                _logger.LogError(ex, "BadRequestException occurred while cancelling Job Order with Id {Id}: {Message}", request.Id, ex.Message);

                return CustomResultResponse.Failure("Handler Validation failed.", ex.ValidationErrors);
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while cancelling Job Order with Id {Id}: {Message}", request.Id, ex.Message);
                return CustomResultResponse.Failure("An unexpected error occurred while processing the request.");
            }


        }
    }
}
