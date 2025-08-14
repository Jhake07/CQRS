using CQRS.Application.Contracts.Interface;
using FluentValidation;

namespace CQRS.Application.Features.BatchSerial
{
    public abstract class BatchSerialCommandValidatorBase<T> : AbstractValidator<T>
    {
        protected readonly IBatchSerialRepository _batchSerialRepository;

        protected BatchSerialCommandValidatorBase(IBatchSerialRepository batchSerialRepository)
        {
            _batchSerialRepository = batchSerialRepository;

            // Shared validation logic for DocNo
            RuleFor(p => GetDocNo(p))
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .NotNull().WithMessage("{PropertyName} should not be null.");
        }

        // Abstract method for extracting shared properties (ContractNo and Doc No)
        protected abstract string GetContractNo(T command);
        protected abstract string GetDocNo(T command);

    }
}
