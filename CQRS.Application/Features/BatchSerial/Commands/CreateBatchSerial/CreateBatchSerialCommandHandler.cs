using AutoMapper;
using CQRS.Application.Contracts.Interface;
using CQRS.Application.Shared.Exceptions;
using CQRS.Application.Shared.Response;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CQRS.Application.Features.BatchSerial.Commands.CreateBatchSerial
{
    public class CreateBatchSerialCommandHandler(IMapper mapper, IBatchSerialRepository batchSerialRepository, IMainSerialRepository mainSerialRepository,
        ILogger<CreateBatchSerialCommandHandler> logger)
        : IRequestHandler<CreateBatchSerialCommand, CustomResultResponse>
    {
        private readonly IMapper _mapper = mapper;
        private readonly IBatchSerialRepository _batchSerialRepository = batchSerialRepository;
        private readonly IMainSerialRepository _mainSerialRepository = mainSerialRepository;
        private readonly ILogger<CreateBatchSerialCommandHandler> _logger = logger;

        public async Task<CustomResultResponse> Handle(CreateBatchSerialCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Validate Incoming Data
                var validator = new CreateBatchSerialCommandValidator(_batchSerialRepository);
                var validationResult = await validator.ValidateAsync(request, cancellationToken);

                if (validationResult.Errors.Count != 0)
                {
                    _logger.LogError("Validation errors occurred while creating batch serial.");
                    throw new BadRequestException("Validation Failed", validationResult);
                }

                // Map the command to the domain entity
                var batchSerial = _mapper.Map<Domain.BatchSerial>(request);

                // Save Batch Serial 
                await _batchSerialRepository.CreateAsync(batchSerial);
                _logger.LogInformation("Batch Serial created successfully with ID: {BatchSerialId}", batchSerial.ContractNo);

                // Generatate Main Serials
                await CreateMainSerialsAsync(request, cancellationToken);
                _logger.LogInformation("Main Serials created successfully for ContractNo: {ContractNo}", request.ContractNo);

                // Return success response
                return new CustomResultResponse
                {
                    IsSuccess = true,
                    Message = "BatchSerial and MainSerials created successfully.",
                    Id = batchSerial.Id.ToString() // Ensure the ID is converted to string if necessary
                };
            }
            catch (BadRequestException ex)
            {
                _logger.LogError(ex, "Validation error occurred for ContractNo: {ContractNo}", request.ContractNo);

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
                // Log and handle exceptions appropriately
                _logger.LogError(ex, "An error occurred while processing CreateBatchSerialCommand for ContractNo: {ContractNo}", request.ContractNo);
                return new CustomResultResponse
                {
                    IsSuccess = false,
                    Message = $"An error occurred: {ex.Message}",
                    Id = null
                };
            }
        }

        public async Task CreateMainSerialsAsync(CreateBatchSerialCommand request, CancellationToken cancellationToken)
        {
            // Initialize the total qty of MainSerials to be created and starting serial#
            int qty = request.BatchQty;
            int startSerialNo = int.Parse(request.StartSNo);

            // Create the list of serial# temporarily for batch processing
            var mainSerials = new List<Domain.MainSerial>();

            // Define constant for the batch size and max retry attempts
            const int batchSize = 300;
            const int maxRetries = 3;

            // Iterate the whole qty and generate each MainSerial
            for (int i = 0; i < qty; i++)
            {
                // Format the serial number to be zero-padded (e.g., 00001)
                var serialNo = string.Format("{0:D5}", startSerialNo + i);

                // Add a new MainSerial to the list with properties populated from the request
                mainSerials.Add(new Domain.MainSerial
                {
                    SerialNo = $"{request.SerialPrefix}{serialNo}",
                    BatchSerial_ContractNo = request.ContractNo,
                    ScanTo = "test" // Replace with Sampling, Line1 or Line2 as needed
                });

                // Check if the batch size is reached or if it's the last iteration 
                if (mainSerials.Count == batchSize || i == qty - 1)
                {
                    bool success = false; // Flag to track if the batch was saved successfully
                    int attempt = 0; // Counter for retry attempts

                    // Retry saving the batch until successful or max attempts reached
                    while (!success && attempt < maxRetries)
                    {
                        try
                        {
                            attempt++;
                            _logger.LogInformation("Attempting to save batch of {BatchSize} MainSerials. Attempt {Attempt}/{MaxRetries}.", mainSerials.Count, attempt, maxRetries);


                            //Save the batch of MainSerials
                            await _mainSerialRepository.BulkCreateAsync(mainSerials, cancellationToken);
                            success = true; // Set success to true if save was successful
                            _logger.LogInformation("Batch of {BatchSize} MainSerials saved successfully.", mainSerials.Count);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error saving batch of MainSerials. Attempt {Attempt}/{MaxRetries}.", attempt, maxRetries);

                            // If an error occurs, wait for a short period before retrying
                            await Task.Delay(1000, cancellationToken);

                            // If maximum retries are reached, save MainSerials individually
                            if (attempt == maxRetries)
                            {
                                _logger.LogCritical("Max retries reached. Fallback to saving individual MainSerials for ContractNo: {ContractNo}.", request.ContractNo);

                                foreach (var serial in mainSerials)
                                {
                                    try
                                    {
                                        // Save individual MainSerial
                                        await _mainSerialRepository.CreateAsync(serial);
                                        _logger.LogInformation("Successfully saved individual MainSerial: {SerialNo}.", serial.SerialNo);
                                    }
                                    catch (Exception innerEx)
                                    {
                                        // Log error for individual MainSerial save failure
                                        _logger.LogError(innerEx, "Failed to save individual MainSerial: {SerialNo}.", serial.SerialNo);
                                    }
                                }
                            }
                        }
                    }
                    // Clear the list after processing the batch to prepare for the next batch
                    mainSerials.Clear();
                }
            }


        }

    }
}
