using CQRS.Application.Contracts.Interface;
using FluentValidation;

namespace CQRS.Application.Features.JobOrder.Commands.Create
{
    public class CreateJobOrderCommandValidator : JobOrderCommandValidatorBase<CreateJobOrderCommand>
    {
        public CreateJobOrderCommandValidator(IJobOrderRepository jobOrderRepository)
            : base(jobOrderRepository)
        {
            RuleFor(p => p.JoNo)
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .NotNull().WithMessage("{PropertyName} should not be null.");

            RuleFor(p => p.JoNo)
                .MustAsync(async (model, jono, _) =>
                    await CheckExistingJobOrder(jono, model.ISNo, model.Line))
                .WithMessage("{PropertyName} already exists.");

            RuleFor(p => p.OrderQty)
                .GreaterThanOrEqualTo(p => p.ProcessOrder)
                .WithMessage("Order quantity must not be less than the processed quantity.");
        }

        protected override string GetContractNo(CreateJobOrderCommand command) => command.BatchSerial_ContractNo;
        protected override int GetOrderQty(CreateJobOrderCommand command) => command.OrderQty;

        private async Task<bool> CheckExistingJobOrder(string jono, string isno, string lineno)
        {
            var exists = await _jobOrderRepository.CheckJobOrderNo(jono, isno, lineno);
            return !exists;
        }
    }
}
