using CQRS.Application.Contracts.Interface;
using CQRS.Application.Shared.Exceptions;
using CQRS.Application.Shared.Response;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CQRS.Application.Features.ScannedUnit.Commands.UpdateScannedUnit.UpdateComponent
{
    public class UpdateComponentsCommandHandler(ILogger<UpdateComponentsCommandHandler> logger, IScannedUnitsRepository scannedUnitsRepository)
        : IRequestHandler<UpdateComponentsCommand, CustomResultResponse>
    {
        private readonly ILogger<UpdateComponentsCommandHandler> _logger = logger;
        private readonly IScannedUnitsRepository _scannedUnitsRepository = scannedUnitsRepository;

        public async Task<CustomResultResponse> Handle(UpdateComponentsCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling command to update components for serial {MainSerial}", request.MainSerial);

            try
            {
                // 1. BUSINESS VALIDATION: Resource Existence Check
                // This check is now back in the handler.
                var unitExists = await _scannedUnitsRepository.UnitExistsAsync(request.MainSerial);

                if (!unitExists)
                {
                    _logger.LogWarning("Scanned Unit with serial {MainSerial} not found for update.", request.MainSerial);
                    // Return a clear failure response with the specific message
                    return CustomResultResponse.Failure($"Unit not found. Serial {request.MainSerial} does not exist in the system.");
                }

                // 2. EXECUTE DATABASE LOGIC
                await _scannedUnitsRepository.UpdateUnitComponentsAsync(
                    request.MainSerial,
                    request.MotherboardSerial,
                    request.PcbiSerial,
                    request.PowerSupplySerial);

                _logger.LogInformation("Successfully updated components for serial {MainSerial}.", request.MainSerial);
                return CustomResultResponse.Success("Unit components updated successfully.", request.MainSerial);
            }
            // Catching BadRequestException (for pipeline validation failures)
            catch (BadRequestException ex)
            {
                _logger.LogWarning(ex, "Validation error occurred for MainSerial: {MainSerial}", request.MainSerial);
                return CustomResultResponse.Failure("Validation failed during component update.", ex.ValidationErrors);
            }
            // Catching generic system exceptions
            catch (Exception ex)
            {
                _logger.LogError(ex, "System failure during component update for {MainSerial}.", request.MainSerial);
                return CustomResultResponse.Failure($"An unexpected server error occurred.");
            }
        }
    }
}