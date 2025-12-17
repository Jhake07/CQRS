using CQRS.Application.Contracts.Interface;
using CQRS.Application.Shared.Exceptions;
using CQRS.Application.Shared.Response;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CQRS.Application.Features.ScannedUnit.Commands.UpdateScannedUnit.UpdateAccessories
{
    public class UpdateAccessoriesCommandHandler(ILogger<UpdateAccessoriesCommandHandler> logger, IScannedUnitsRepository scannedUnitsRepository)
        : IRequestHandler<UpdateAccessoriesCommand, CustomResultResponse>
    {
        private readonly ILogger<UpdateAccessoriesCommandHandler> _logger = logger;
        private readonly IScannedUnitsRepository _scannedUnitsRepository = scannedUnitsRepository;

        public async Task<CustomResultResponse> Handle(UpdateAccessoriesCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling command to update accessories for serial {MainSerial}", request.mainSerial);

            try
            {
                // 1. BUSINESS VALIDATION: Resource Existence Check
                // This check is now back in the handler.
                var unitExists = await _scannedUnitsRepository.UnitExistsAsync(request.mainSerial);

                if (!unitExists)
                {
                    _logger.LogWarning("Scanned Unit with serial {MainSerial} not found for update.", request.mainSerial);
                    // Return a clear failure response with the specific message
                    return CustomResultResponse.Failure($"Unit not found. Serial {request.mainSerial} does not exist in the system.");
                }

                // 2. EXECUTE DATABASE LOGIC
                await _scannedUnitsRepository.UpdateUnitAccessoriesAsync(
                    request.mainSerial,
                    request.accessoriesSerial);

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
