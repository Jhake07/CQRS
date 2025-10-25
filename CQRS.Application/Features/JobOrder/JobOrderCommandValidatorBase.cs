using CQRS.Application.Contracts.Interface;
using FluentValidation;

public abstract class JobOrderCommandValidatorBase<TCommand> : AbstractValidator<TCommand>
{
    protected readonly IJobOrderRepository _jobOrderRepository;

    protected JobOrderCommandValidatorBase(IJobOrderRepository jobOrderRepository)
    {
        _jobOrderRepository = jobOrderRepository;

        RuleFor(p => p)
            .MustAsync((command, cancellationToken) =>
                OrderQtyIsValid(command, cancellationToken))
            .WithMessage("Order quantity exceeds available batch quantity.");
    }

    protected abstract string GetContractNo(TCommand command);
    protected abstract int GetOrderQty(TCommand command);

    private async Task<bool> OrderQtyIsValid(TCommand command, CancellationToken cancellationToken)
    {
        var contractNo = GetContractNo(command);
        var orderQty = GetOrderQty(command);

        if (string.IsNullOrWhiteSpace(contractNo)) return false;

        return await _jobOrderRepository.CheckOrderQty(contractNo, orderQty);
    }
}
