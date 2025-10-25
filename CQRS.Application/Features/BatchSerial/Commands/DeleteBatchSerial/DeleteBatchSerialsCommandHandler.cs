using CQRS.Application.Constants;
using CQRS.Application.Contracts.Interface;
using CQRS.Application.Shared.Exceptions;
using CQRS.Application.Shared.Response;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CQRS.Application.Features.BatchSerial.Commands.DeleteBatchSerial
{
    public class DeleteBatchSerialsCommandHandler(
       ILogger<DeleteBatchSerialsCommandHandler> logger,
       IBatchSerialRepository batchSerialRepository) : IRequestHandler<DeleteBatchSerialsCommand, CustomResultResponse>
    {
        private readonly ILogger<DeleteBatchSerialsCommandHandler> _logger = logger;
        private readonly IBatchSerialRepository _batchSerialRepository = batchSerialRepository;


        public async Task<CustomResultResponse> Handle(DeleteBatchSerialsCommand request, CancellationToken cancellationToken)
        {
            try
            {

                var batchSerial = await _batchSerialRepository.GetBatchSerialsById(request.Id);
                if (batchSerial == null)
                {
                    _logger.LogWarning("Batch Contract details with Id {Id} not found.", request.Id);
                    return new CustomResultResponse
                    {
                        IsSuccess = false,
                        Message = $"Batch Contract with Id {request.Id} was not found."
                    };
                }

                // Only Batch Serial with status 'Open' can be cancelled
                if (!string.Equals(batchSerial.Status, BatchStatus.Open, StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogInformation("Batch Contract {ContractNo} is already in '{Status}' status. No action taken.", batchSerial.ContractNo, batchSerial.Status);

                    return new CustomResultResponse
                    {
                        IsSuccess = false,
                        Message = $"Batch Contract is already in '{batchSerial.Status}' status.",
                        Id = batchSerial.ContractNo.ToString()
                    };
                }

                // Explicitly set the cancellation status 
                batchSerial.Status = "Cancelled";

                // Update the batch status to Cancelled
                await _batchSerialRepository.UpdateAsync(batchSerial);

                _logger.LogInformation("Batch Contract successfully cancelled");

                return new CustomResultResponse
                {
                    IsSuccess = true,
                    Message = $"Batch Contract  '{batchSerial.ContractNo}' successfully cancelled",
                    Id = batchSerial.ContractNo.ToString()
                };
            }
            catch (BadRequestException ex)
            {
                _logger.LogError(ex, "Deletion error occurred for Transaction: {Id}", request.Id);

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
                // Log and handle exceptions appropriately
                _logger.LogError(ex, "An error occurred while updating the details for Batch Contract: {Id}", request.Id);
                return new CustomResultResponse
                {
                    IsSuccess = false,
                    Message = $"An error occurred: {ex.Message}",
                    Id = null
                };
            }
        }
    }
}
