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
            RuleFor(p => p)
             .MustAsync((command, cancellationToken) =>
                ContractNoIsValid(command, cancellationToken))
             .WithMessage("ContractNo already exists.");
        }

        protected override string GetContractNo(UpdateBatchSerialCommand command)
        {
            return command.ContractNo;
        }
        protected override string GetDocNo(UpdateBatchSerialCommand command)
        {
            return command?.DocNo ?? string.Empty; // Use null coalescing operator for safety
        }
        private async Task<bool> ContractNoIsValid(UpdateBatchSerialCommand command, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(command.ContractNo)) return false;

            var existing = await _batchSerialRepository.GetBatchSerialsById(command.Id);
            if (existing == null) return true;

            if (existing.ContractNo == command.ContractNo) return true;

            var exists = await _batchSerialRepository.CheckBatchContractNo(command.ContractNo);
            return !exists;
        }

    }
}
