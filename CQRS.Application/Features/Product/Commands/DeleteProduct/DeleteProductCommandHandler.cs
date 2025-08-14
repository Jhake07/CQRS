using CQRS.Application.Contracts.Interface;
using CQRS.Application.Shared.Exceptions;
using CQRS.Application.Shared.Response;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CQRS.Application.Features.Product.Commands.DeleteProduct
{
    public class DeleteProductCommandHandler(ILogger<DeleteProductCommandHandler> logger, IProductRepository productRepository)
        : IRequestHandler<DeleteProductCommand, CustomResultResponse>
    {
        private readonly ILogger _logger = logger;
        private readonly IProductRepository _productRepository = productRepository;

        public async Task<CustomResultResponse> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var product = await _productRepository.GetByIdAsync(request.Id);

                if (product == null)
                {
                    _logger.LogWarning("Product with Id {Id} not found.", request.Id);
                    return new CustomResultResponse
                    {
                        IsSuccess = false,
                        Message = $"Product with Id {request.Id} was not found."
                    };
                }

                // Only products with status 'Active' can be deleted
                if (!string.Equals(product.Stats, "Active", StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogInformation("Product {ModelCode} is already in '{Status}' status. No action taken.", product.ModelCode, product.Stats);
                    return new CustomResultResponse
                    {
                        IsSuccess = true,
                        Message = $"Product is already in '{product.Stats}' status.",
                        Id = product.ModelCode
                    };
                }

                // Explicitly set the Inactive status
                product.Stats = "Inactive";

                // Update the product status to Inactive
                await _productRepository.UpdateAsync(product);

                _logger.LogInformation("Product successfully updated status to Inactive");

                return new CustomResultResponse
                {
                    IsSuccess = true,
                    Message = $"Product Model: '{product.ModelCode}' successfully inactive",
                    Id = product.ModelCode.ToString()
                };
            }
            catch (BadRequestException ex)
            {
                _logger.LogError(ex, "Validation error occurred");
                return new CustomResultResponse
                {
                    IsSuccess = false,
                    Message = "Validation failed",
                    ValidationErrors = ex.ValidationErrors
                };
            }
            catch (Exception ex)
            {
                // Log and handle exceptions appropriately
                _logger.LogError(ex, "An error occurred while updating the product details");
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
