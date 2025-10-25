using AutoMapper;
using CQRS.Application.Contracts.Interface;
using CQRS.Application.Shared.Exceptions;
using CQRS.Application.Shared.Response;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CQRS.Application.Features.JobOrder.Commands.Update
{
    public class UpdateJobOrderCommandHandler(
        IMapper mapper,
        ILogger<UpdateJobOrderCommandHandler> logger,
        IJobOrderRepository jobOrderRepository,
        IBatchSerialRepository batchSerialRepository)
        : IRequestHandler<UpdateJobOrderCommand, CustomResultResponse>
    {
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<UpdateJobOrderCommandHandler> _logger = logger;
        private readonly IJobOrderRepository _jobOrderRepository = jobOrderRepository;
        private readonly IBatchSerialRepository _batchSerialRepository = batchSerialRepository;

        public async Task<CustomResultResponse> Handle(UpdateJobOrderCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var joborder = await _jobOrderRepository.GetByIdAsync(request.Id);
                if (joborder is null)
                {
                    _logger.LogWarning("Job Order with JoNo {JoNo} not found.", request.JoNo);
                    return CustomResultResponse.Failure($"Job Order with JoNo {request.JoNo} was not found.");
                }

                var batchserial = await _batchSerialRepository.GetBatchSerialsByContractNo(request.BatchSerial_ContractNo);
                if (batchserial is null)
                {
                    _logger.LogWarning("Batch Serial with ContractNo {ContractNo} not found.", request.BatchSerial_ContractNo);
                    return CustomResultResponse.Failure($"Batch Serial with ContractNo {request.BatchSerial_ContractNo} was not found.");
                }

                var validator = new UpdateJobOrderCommandValidator(_jobOrderRepository);
                var validationResult = await validator.ValidateAsync(request, cancellationToken);
                if (!validationResult.IsValid)
                {
                    _logger.LogWarning("Validation failed for UpdateJobOrderCommand: {Errors}", validationResult.Errors);
                    throw new BadRequestException("Validation failed", validationResult);
                }

                if (request.OrderQty != joborder.OrderQty)
                {
                    var qtyDiff = request.OrderQty - joborder.OrderQty;

                    if (!TryUpdateBatchSerial(batchserial, qtyDiff, out string errorMessage))
                    {
                        _logger.LogWarning(errorMessage);
                        return CustomResultResponse.Failure("Order quantity exceeds available batch quantity.");
                    }

                    await _batchSerialRepository.UpdateAsync(batchserial);
                }
                else
                {
                    _logger.LogInformation("No change in Order Quantity for JoNo: {JoNo}. Skipping Batch Serial update.", request.JoNo);
                }

                _mapper.Map(request, joborder);
                await _jobOrderRepository.UpdateAsync(joborder);

                _logger.LogInformation("Job Order successfully updated for JoNo: {JoNo}", request.JoNo);
                return CustomResultResponse.Success("Job Order details successfully updated.", request.JoNo);
            }
            catch (BadRequestException ex)
            {
                _logger.LogError(ex, "Validation error occurred for JoNo: {JoNo}", request.JoNo);
                return CustomResultResponse.Failure("Handler Validation failed.", ex.ValidationErrors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating Job Order for JoNo: {JoNo}", request.JoNo);
                return CustomResultResponse.Failure("An error occurred while processing your request.");
            }
        }

        private static bool TryUpdateBatchSerial(Domain.BatchSerial batch, int qtyDiff, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (qtyDiff > 0)
            {
                if (qtyDiff > batch.RemainingQty)
                {
                    errorMessage = $"Insufficient left quantity in Batch Serial for ContractNo: {batch.ContractNo}";
                    return false;
                }

                batch.OrderQty += qtyDiff;
                batch.RemainingQty -= qtyDiff;
            }
            else if (qtyDiff < 0)
            {
                batch.OrderQty += qtyDiff;
                batch.RemainingQty -= qtyDiff; // subtracting a negative = adding

                // Cap RemainingQty to BatchQty
                batch.RemainingQty = Math.Min(batch.RemainingQty, batch.BatchQty);
            }

            batch.Status = batch.RemainingQty <= 0 ? "Out of stock" : "Open";
            return true;
        }
    }

}
