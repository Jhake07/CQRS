using CQRS.Application.Contracts.Interface;
using FluentValidation;

namespace CQRS.Application.Features.Product.Commands.CreateProduct
{
    public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        private readonly IProductRepository _productRepository;

        public CreateProductCommandValidator(IProductRepository productRepository)
        {
            _productRepository = productRepository;
            // Shared validation logic for ProductCode
            RuleFor(p => GetProductCode(p))
                .MustAsync(CheckExistingProductCode)
                .WithMessage("{PropertyName} already exists.")
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .NotNull().WithMessage("{PropertyName} should not be null.");

            RuleFor(p => p.ModelCode)
               .NotEmpty().WithMessage("{PropertyName} is required.")
               .NotNull().WithMessage("{PropertyName} should not be null.");

            RuleFor(p => p.Description)
               .NotEmpty().WithMessage("{PropertyName} is required.")
               .NotNull().WithMessage("{PropertyName} should not be null.");

            RuleFor(p => p.Brand)
               .NotEmpty().WithMessage("{PropertyName} is required.")
               .NotNull().WithMessage("{PropertyName} should not be null.");

            RuleFor(p => p.DefaultJOQty)
               .NotEmpty().WithMessage("{PropertyName} is required.")
               .NotNull().WithMessage("{PropertyName} should not be null.");

            RuleFor(p => p.Accessories)
               .NotEmpty().WithMessage("{PropertyName} is required.")
               .NotNull().WithMessage("{PropertyName} should not be null.");

            RuleFor(p => p.Stats)
               .NotEmpty().WithMessage("{PropertyName} is required.")
               .NotNull().WithMessage("{PropertyName} should not be null.");
        }

        private async Task<bool> CheckExistingProductCode(string productCode, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(productCode)) return true;
            var exists = await _productRepository.CheckProductCodeAsync(productCode);
            return !exists; // Ensure ProductCode does not exist
        }

        private static string GetProductCode(CreateProductCommand command)
        {
            return command.ModelCode;

        }
    }
}
