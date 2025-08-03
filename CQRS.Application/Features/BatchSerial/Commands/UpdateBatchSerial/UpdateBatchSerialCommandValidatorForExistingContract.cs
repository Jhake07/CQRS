using CQRS.Application.Contracts.Interface;
using FluentValidation;

namespace CQRS.Application.Features.BatchSerial.Commands.UpdateBatchSerial
{
    public class UpdateBatchSerialCommandValidatorForExistingContract : AbstractValidator<UpdateBatchSerialCommand>
    {
        public UpdateBatchSerialCommandValidatorForExistingContract(IBatchSerialRepository batchSerialRepository)
        {
            RuleFor(p => p.ContractNo)
                .NotEmpty().WithMessage("{PropertyName} is required.");
            //.NotNull().WithMessage("{PropertyName} is required.");

            RuleFor(p => p.DocNo)
                .NotEmpty().WithMessage("{PropertyName} is required.");
            //.NotNull().WithMessage("{PropertyName} is required.");
        }
    }
}
