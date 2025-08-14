using CQRS.Application.Contracts.Interface;
using FluentValidation;

namespace CQRS.Application.Features.Product.Commands.UpdateProduct
{
    public class UpdateProductCommandValidator : ProductCommandValidatorBase<UpdateProductCommand>
    {
        public UpdateProductCommandValidator(IProductRepository productRepository)
            : base(productRepository)
        {
            RuleFor(p => p)
                         .MustAsync((command, cancellationToken) =>
                            ModelCodeIsValid(command))
                         .WithMessage("Model code already exists.");
        }
        protected override string GetModelCode(UpdateProductCommand command) => command.ModelCode;

        private async Task<bool> ModelCodeIsValid(UpdateProductCommand command)
        {
            if (string.IsNullOrWhiteSpace(command.ModelCode)) return false;

            var existing = await _productRepository.FindProductByCodeAsync(command.ModelCode);

            // Allow if no product has this code
            if (existing == null) return true;

            // Allow if it's the same product being updated
            if (existing.Id == command.Id) return true;

            // Block if another product already uses this code
            return false;

        }
    }

}
