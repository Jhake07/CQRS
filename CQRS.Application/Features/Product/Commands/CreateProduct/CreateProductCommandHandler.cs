using AutoMapper;
using CQRS.Application.Contracts.Interface;
using CQRS.Application.Shared.Exceptions;
using CQRS.Application.Shared.Response;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CQRS.Application.Features.Product.Commands.CreateProduct
{
    public class CreateProductCommandHandler(IMapper mapper, IProductRepository productRepository, ILogger<CreateProductCommandHandler> logger)
        :
        IRequestHandler<CreateProductCommand, CustomResultResponse>
    {
        private readonly IMapper _mapper = mapper;
        private readonly IProductRepository _productRepository = productRepository;
        private readonly ILogger<CreateProductCommandHandler> _logger = logger;

        public async Task<CustomResultResponse> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Validate Incoming Data
                var validator = new CreateProductCommandValidator(_productRepository);
                var validationResult = await validator.ValidateAsync(request, cancellationToken);

                if (validationResult.Errors.Count != 0)
                {
                    _logger.LogError("Validation errors occurred while creating product.");
                    throw new BadRequestException("Validation Failed", validationResult);
                }

                // Map the command to the domain entity
                var product = _mapper.Map<Domain.Product>(request);

                // Save Product
                await _productRepository.CreateAsync(product);
                _logger.LogInformation("Product created successfully with ModelCode: {ModelCode}", product.ModelCode);

                return new CustomResultResponse
                {
                    IsSuccess = true,
                    Message = "Product created successfully.",
                    Id = product.Id.ToString() // Ensure the ID is converted to string if necessary
                };
            }
            catch (BadRequestException ex)
            {
                _logger.LogError(ex, "Validation error occurred for ModelCode: {ModelCode}", request.ModelCode);
                return new CustomResultResponse
                {
                    IsSuccess = false,
                    Message = "Handler Validation failed.",
                    ValidationErrors = ex.ValidationErrors // Return structured validation errors
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating product with ModelCode: {ModelCode}", request.ModelCode);
                return new CustomResultResponse
                {
                    IsSuccess = false,
                    Message = "An error occurred while creating the product."
                };
            }
        }
    }
}
