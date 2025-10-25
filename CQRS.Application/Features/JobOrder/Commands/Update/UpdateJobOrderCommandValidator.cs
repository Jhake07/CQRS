using CQRS.Application.Contracts.Interface;
using FluentValidation;

namespace CQRS.Application.Features.JobOrder.Commands.Update
{
    public class UpdateJobOrderCommandValidator : JobOrderCommandValidatorBase<UpdateJobOrderCommand>
    {
        public UpdateJobOrderCommandValidator(IJobOrderRepository jobOrderRepository)
            : base(jobOrderRepository)
        {
            RuleFor(p => p.JoNo)
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .NotNull().WithMessage("{PropertyName} should not be null.");

            RuleFor(p => p.OrderQty)
                .GreaterThanOrEqualTo(p => p.ProcessOrder)
                .WithMessage("Order quantity must not be less than the processed quantity.");

            RuleFor(p => p)
                .MustAsync((command, cancellationToken) =>
                    JobOrderIsUpdatable(command, cancellationToken))
                .WithMessage("Completed job orders cannot be updated.");
        }

        protected override string GetContractNo(UpdateJobOrderCommand command) => command.BatchSerial_ContractNo;
        protected override int GetOrderQty(UpdateJobOrderCommand command) => command.OrderQty;

        private async Task<bool> JobOrderIsUpdatable(UpdateJobOrderCommand command, CancellationToken cancellationToken)
        {
            return !await _jobOrderRepository.CheckJoStatus(command.Id);
        }
    }
}