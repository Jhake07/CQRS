using AutoMapper;
using CQRS.Application.Contracts.Interface;
using CQRS.Application.Shared.Exceptions;
using CQRS.Application.Shared.Response;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CQRS.Application.Features.ScannedUnit.Commands.CreateScannedUnit
{
    public class CreateScannedUnitCommandHandler(IMapper mapper, IScannedUnitsRepository scannedUnitsRepository, IMainSerialRepository mainSerialRepository, ILogger<CreateScannedUnitCommandHandler> logger)
        :
        IRequestHandler<CreateScannedUnitCommand, CustomResultResponse>
    {
        private readonly IMapper _mapper = mapper;
        private readonly IScannedUnitsRepository _scannedUnitsRepository = scannedUnitsRepository;
        private readonly IMainSerialRepository _mainSerialRepository = mainSerialRepository;
        private readonly ILogger<CreateScannedUnitCommandHandler> _logger = logger;

        public async Task<CustomResultResponse> Handle(CreateScannedUnitCommand request, CancellationToken cancellationToken)
        {
            try
            {
                return await HandleScannedUnitCreationAsync(request, cancellationToken);
            }
            catch (BadRequestException ex)
            {
                return HandleValidationError(ex);
            }
            catch (Exception ex)
            {
                return HandleError(ex, "An unexpected error occurred while creating scanned unit: {Message}");
            }
        }

        private async Task<CustomResultResponse> HandleScannedUnitCreationAsync(CreateScannedUnitCommand request, CancellationToken cancellationToken)
        {
            // Validate incoming data
            var validator = new CreateScannedUnitCommandValidator(_scannedUnitsRepository);
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (validationResult.Errors.Count > 0)
            {
                _logger.LogError("Validation failed for CreateScannedUnitCommand: {Errors}", validationResult.Errors);
                throw new BadRequestException("Validation failed", validationResult);
            }

            // Get the batch contract as per the given main serial 
            var contractNo = await _scannedUnitsRepository.GetMainSerialContractNo(request.MainSerial);

            // Get the first in progress status Job order number from the batch contract number 
            var jobOrderNumber = await _scannedUnitsRepository.GetJobOrderNumberAsync(contractNo.BatchSerial_ContractNo);

            // Map the command to the domain model
            var scannedUnit = _mapper.Map<Domain.ScannedUnits>(request);

            // Map ContractNo, JobOrderNo, and MainSerial into the entity           
            scannedUnit.ContractNo = contractNo.BatchSerial_ContractNo;
            scannedUnit.JoNo = jobOrderNumber;
            scannedUnit.MainSerial = request.MainSerial;

            // Save the new scanned unit to the database
            await _scannedUnitsRepository.CreateAsync(scannedUnit);

            // Update JONo from the Mainserial table to avoid the duplicate entry error for the same main serial number
            await _mainSerialRepository.UpdateSerialJoNo(scannedUnit.MainSerial, scannedUnit.JoNo);

            _logger.LogInformation("Scanned unit created successfully with MainSerial: {MainSerial}", scannedUnit.MainSerial);
            return CustomResultResponse.Success("Scanned unit created successfully", scannedUnit.Id.ToString());
        }

        // Extracted error handling methods
        private CustomResultResponse HandleValidationError(BadRequestException ex)
        {
            _logger.LogError(ex, "Bad request error while creating scanned unit: {Message}", ex.Message);
            return CustomResultResponse.Failure(ex.Message, ex.ValidationErrors);
        }

        private CustomResultResponse HandleError(Exception ex, string message)
        {
            _logger.LogError(ex, message, ex.Message);
            return CustomResultResponse.Failure("An unexpected error occurred. Please try again later.");
        }



    }
}
