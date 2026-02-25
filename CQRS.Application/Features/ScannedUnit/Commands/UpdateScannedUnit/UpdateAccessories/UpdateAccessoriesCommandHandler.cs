using CQRS.Application.Contracts.Interface;
using CQRS.Application.Shared.Exceptions;
using CQRS.Application.Shared.Response;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CQRS.Application.Features.ScannedUnit.Commands.UpdateScannedUnit.UpdateAccessories
{
    public class UpdateAccessoriesCommandHandler(ILogger<UpdateAccessoriesCommandHandler> logger, IScannedUnitsRepository scannedUnitsRepository, IMainSerialRepository mainSerialRepository, IJobOrderRepository jobOrderRepository)
        : IRequestHandler<UpdateAccessoriesCommand, CustomResultResponse>
    {
        private readonly ILogger<UpdateAccessoriesCommandHandler> _logger = logger;
        private readonly IScannedUnitsRepository _scannedUnitsRepository = scannedUnitsRepository;
        private readonly IMainSerialRepository _mainSerialRepository = mainSerialRepository;
        private readonly IJobOrderRepository _jobOrderRepository = jobOrderRepository;

        public async Task<CustomResultResponse> Handle(UpdateAccessoriesCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling command to update accessories for serial {MainSerial}", request.mainSerial);

            try
            {
                // BUSINESS VALIDATION: Resource Existence Check
                // This check is now back in the handler.
                var unitExists = await _scannedUnitsRepository.UnitExistsAsync(request.mainSerial);

                if (!unitExists)
                {
                    _logger.LogWarning("Scanned Unit with serial {MainSerial} not found for update.", request.mainSerial);
                    // Return a clear failure response with the specific message
                    return CustomResultResponse.Failure($"Unit not found. Serial {request.mainSerial} does not exist in the system.");
                }

                // Get the main unit's Job Order ID for later use in updating the ProcessOrder quantity
                var jono = await _mainSerialRepository.GetJobOrderIdByMainSerialAsync(request.mainSerial);

                // EXECUTE DATABASE LOGIC
                await _scannedUnitsRepository.UpdateUnitAccessoriesAsync(
                    request.mainSerial,
                    request.accessoriesSerial);

                // Add 1 to the ProcessOrder quantity for the Job Order
                await _jobOrderRepository.AddProcessOrderQty(jono);

                // Check if the Job Order is now completed after updating the ProcessOrder quantity
                var isCompleted = await _jobOrderRepository.GetJobOrderCount(jono);

                if (isCompleted.OrderQty == isCompleted.ProcessOrder)
                // Update the JobOrder Status to "Completed" if the order quantity matches the processed order quantity
                {
                    await _jobOrderRepository.UpdateJoStatus(jono);
                }

                _logger.LogInformation("Successfully updated the accessories for serial {MainSerial}.", request.mainSerial);
                return CustomResultResponse.Success("Unit accessories updated successfully.", request.mainSerial);
            }
            // Catching BadRequestException (for pipeline validation failures)
            catch (BadRequestException ex)
            {
                _logger.LogWarning(ex, "Validation error occurred for MainSerial: {MainSerial}", request.mainSerial);
                return CustomResultResponse.Failure("Validation failed during accessories update.", ex.ValidationErrors);
            }
            // Catching generic system exceptions
            catch (Exception ex)
            {
                _logger.LogError(ex, "System failure during accessories update for {MainSerial}.", request.mainSerial);
                return CustomResultResponse.Failure($"An unexpected server error occurred.");
            }
        }
    }
}
