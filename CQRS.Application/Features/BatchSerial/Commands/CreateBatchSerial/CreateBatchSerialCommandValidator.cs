using CQRS.Application.Contracts.Interface;
using FluentValidation;

namespace CQRS.Application.Features.BatchSerial.Commands.CreateBatchSerial
{
    public class CreateBatchSerialCommandValidator : BatchSerialCommandValidatorBase<CreateBatchSerialCommand>
    {
        public CreateBatchSerialCommandValidator(IBatchSerialRepository batchSerialRepository)
           : base(batchSerialRepository)
        {
            // Add additional validation rules specific to CreateBatchSerialCommand

            RuleFor(p => p.BatchQty)
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .NotNull().WithMessage("{PropertyName} should not be null.")
                .GreaterThan(0).WithMessage("{PropertyName} must be more than 0");

            RuleFor(p => p.SerialPrefix)
                .MustAsync(CheckExistingSerialPrefix)
                .WithMessage("{PropertyName} already exists.")
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .NotNull().WithMessage("{PropertyName} should not be null.");

            RuleFor(p => p.StartSNo)
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .NotNull().WithMessage("{PropertyName} should not be null.")
                .Matches(@"^\d+$").WithMessage("Starting Serial# must be numeric."); // Add validation for numeric strings;

            RuleFor(p => p.EndSNo)
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .NotNull().WithMessage("{PropertyName} should not be null.")
                .Matches(@"^\d+$").WithMessage("Ending Serial# must be numeric."); // Add validation for numeric strings;

            RuleFor(p => p.Item_ModelCode)
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .NotNull().WithMessage("{PropertyName} should not be null.");
        }

        protected override string GetContractNo(CreateBatchSerialCommand command)
        {
            return command.ContractNo;
        }
        protected override string GetDocNo(CreateBatchSerialCommand command)
        {
            return command.DocNo;
        }
        private async Task<bool> CheckExistingSerialPrefix(string serialPrefix, CancellationToken cancellationToken)
        {
            var exists = await _batchSerialRepository.CheckMainSerialPrefix(serialPrefix);
            return !exists; // Ensure SerialPrefix does not exist
        }
    }
}
