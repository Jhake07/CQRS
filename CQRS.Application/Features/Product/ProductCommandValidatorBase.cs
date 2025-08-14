using CQRS.Application.Contracts.Interface;
using FluentValidation;

namespace CQRS.Application.Features.Product
{
    public abstract class ProductCommandValidatorBase<T> : AbstractValidator<T>
    {
        protected readonly IProductRepository _productRepository;

        protected ProductCommandValidatorBase(IProductRepository productRepository)
        {
            _productRepository = productRepository;

            // Shared validation logic for Model Code
            RuleFor(p => GetModelCode(p))
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .NotNull().WithMessage("{PropertyName} should not be null.");

        }

        // Abstract method for extracting shared properties Model Code
        protected abstract string GetModelCode(T command);

    }
}
