using AutoMapper;
using CQRS.Application.Contracts.Interface;
using CQRS.Application.Shared.Exceptions;
using CQRS.Application.Shared.Response;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CQRS.Application.Features.JobOrder.Commands.Create
{
    public class CreateJobOrderCommandHandler(
        IMapper mapper,
        IJobOrderRepository jobOrderRepository,
        IBatchSerialRepository batchSerialRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateJobOrderCommandHandler> logger)
        : IRequestHandler<CreateJobOrderCommand, CustomResultResponse>
    {
        private readonly IMapper _mapper = mapper;
        private readonly IJobOrderRepository _jobOrderRepository = jobOrderRepository;
        private readonly IBatchSerialRepository _batchSerialRepository = batchSerialRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<CreateJobOrderCommandHandler> _logger = logger;

        public async Task<CustomResultResponse> Handle(CreateJobOrderCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Validate data
                var validator = new CreateJobOrderCommandValidator(_jobOrderRepository);
                var validationResult = await validator.ValidateAsync(request, cancellationToken);

                if (validationResult.Errors.Count != 0)
                {
                    _logger.LogError("Validation errors occurred while creating job order.");
                    throw new BadRequestException("Validation Failed", validationResult);
                }

                // Map the command to the domain entity
                var jobOrder = _mapper.Map<Domain.JobOrder>(request);

                // Fetch BatchSerial to update LeftQty
                var batchSerial = await _batchSerialRepository.GetBatchSerialsByContractNo(request.BatchSerial_ContractNo);
                if (batchSerial == null)
                {
                    _logger.LogError("BatchSerial not found for ContractNo: {ContractNo}", request.BatchSerial_ContractNo);
                    throw new BadRequestException("BatchSerial not found.");
                }

                var remainingQty = batchSerial.RemainingQty;

                // Ensure sufficient quantity
                if (request.OrderQty > remainingQty)
                {
                    _logger.LogError("Insufficient left quantity for ContractNo: {ContractNo}", request.BatchSerial_ContractNo);
                    throw new BadRequestException("Order quantity exceeds available batch quantity.");
                }

                // Update OrderQty and RemainingQty to reflect deduction
                batchSerial.OrderQty += request.OrderQty;
                batchSerial.RemainingQty -= request.OrderQty;

                // If batch is fully utilized, update status
                if (batchSerial.RemainingQty <= 0)
                {
                    batchSerial.Status = "Out of stock";
                }


                // Save Job Order
                await _jobOrderRepository.CreateAsync(jobOrder);
                _logger.LogInformation("Job Order created successfully with ID: {JobOrderId}", jobOrder.Id);

                // Commit both changes
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return new CustomResultResponse
                {
                    IsSuccess = true,
                    Message = "Job Order created successfully.",
                    Id = jobOrder.Id.ToString()
                };
            }
            catch (BadRequestException ex)
            {
                _logger.LogError(ex, "Validation error occurred for JobOrderNo: {JobOrderNo}", request.JoNo);
                return new CustomResultResponse
                {
                    IsSuccess = false,
                    Message = "Handler Validation failed.",
                    ValidationErrors = ex.ValidationErrors,
                    Id = null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing CreateJobOrderCommand for JobOrderNo: {JobOrderNo}", request.JoNo);
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