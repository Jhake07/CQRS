using AutoMapper;
using CQRS.Application.Contracts.Interface;
using CQRS.Application.Shared.Exceptions;
using CQRS.Application.Shared.Response;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CQRS.Application.Features.Product.Commands.UpdateProduct
{
    public class UpdateProductCommandHandler(IMapper mapper, ILogger<UpdateProductCommandHandler> logger, IProductRepository productRepository)
        : IRequestHandler<UpdateProductCommand, CustomResultResponse>
    {
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<UpdateProductCommandHandler> _logger = logger;
        private readonly IProductRepository _productRepository = productRepository;

        public async Task<CustomResultResponse> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if product exists
                var product = await _productRepository.GetByIdAsync(request.Id);
                if (product == null)
                {
                    _logger.LogWarning("Product with ModelCode {ModelCode} not found.", request.ModelCode);
                    return new CustomResultResponse
                    {
                        IsSuccess = false,
                        Message = $"Product with ModelCode {request.ModelCode} was not found."
                    };
                }

                // Unified validation
                var validator = new UpdateProductCommandValidator(_productRepository);
                var validationResult = await validator.ValidateAsync(request, cancellationToken);

                if (!validationResult.IsValid)
                {
                    _logger.LogWarning("Validation failed for UpdateProductCommand: {Errors}", validationResult.Errors);
                    throw new BadRequestException("Validation failed", validationResult);
                }

                // Map Entity updates                
                _mapper.Map(request, product);

                // check if the updated code is existing, if not do the update
                await _productRepository.UpdateAsync(product);

                _logger.LogInformation("Product successfully updated for Model: {ModelCode}", request.ModelCode);

                return new CustomResultResponse
                {
                    IsSuccess = true,
                    Message = "Product details successfully updated.",
                    Id = request.ModelCode
                };
            }
            catch (BadRequestException ex)
            {
                _logger.LogError(ex, "Validation error occurred for ModelCode: {ModelCode}", request.ModelCode);

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
                _logger.LogError(ex, "An error occurred while updating the product: {ModelCode}", request.ModelCode);
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
