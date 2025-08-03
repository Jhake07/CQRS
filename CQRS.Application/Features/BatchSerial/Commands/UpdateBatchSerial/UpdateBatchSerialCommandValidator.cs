using CQRS.Application.Contracts.Interface;
using FluentValidation;

namespace CQRS.Application.Features.BatchSerial.Commands.UpdateBatchSerial
{
    public class UpdateBatchSerialCommandValidator : BatchSerialCommandValidatorBase<UpdateBatchSerialCommand>
    {
        public UpdateBatchSerialCommandValidator(IBatchSerialRepository batchSerialRepository)
            : base(batchSerialRepository)
        {
            // Add additional validation rules specific to UpdateBatchSerialCommand (if any)
            RuleFor(p => p.ContractNo)
            .NotEmpty().WithMessage("{PropertyName} is required.")
            .NotNull().WithMessage("{PropertyName} is required.");
        }

        protected override string GetContractNo(UpdateBatchSerialCommand command)
        {
            return command.ContractNo;
        }
        protected override string GetDocNo(UpdateBatchSerialCommand command)
        {
            return command?.DocNo ?? string.Empty; // Use null coalescing operator for safety
        }
    }
}
