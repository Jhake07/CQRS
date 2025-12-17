using CQRS.Application.Contracts.Interface;
using CQRS.Application.Shared.Exceptions;
using CQRS.Application.Shared.Response;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CQRS.Application.Features.ScannedUnit.Commands.UpdateScannedUnit.UpdateTag
{
    public class UpdateTagCommandHandler(ILogger<UpdateTagCommandHandler> logger, IScannedUnitsRepository scannedUnitsRepository)
        : IRequestHandler<UpdateTagCommand, CustomResultResponse>
    {
        private readonly ILogger<UpdateTagCommandHandler> _logger = logger;
        private readonly IScannedUnitsRepository _scannedUnitsRepository = scannedUnitsRepository;

        public async Task<CustomResultResponse> Handle(UpdateTagCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling command to update tag for serial {MainSerial}", request.MainSerial);

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
                await _scannedUnitsRepository.UpdateUnitTagAsync(
                    request.MainSerial,
                    request.NewTagNo);

                _logger.LogInformation("Successfully updated the Tag for serial {MainSerial}.", request.MainSerial);
                return CustomResultResponse.Success("Unit tag updated successfully.", request.MainSerial);
            }
            // Catching BadRequestException (for pipeline validation failures)
            catch (BadRequestException ex)
            {
                _logger.LogWarning(ex, "Validation error occurred for MainSerial: {MainSerial}", request.MainSerial);
                return CustomResultResponse.Failure("Validation failed during tagging update.", ex.ValidationErrors);
            }
            // Catching generic system exceptions
            catch (Exception ex)
            {
                _logger.LogError(ex, "System failure during tagging update for {MainSerial}.", request.MainSerial);
                return CustomResultResponse.Failure($"An unexpected server error occurred.");
            }
        }
    }
}
