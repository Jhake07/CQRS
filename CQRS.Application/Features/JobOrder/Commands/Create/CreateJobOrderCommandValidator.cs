using CQRS.Application.Contracts.Interface;
using FluentValidation;

namespace CQRS.Application.Features.JobOrder.Commands.Create
{
    public class CreateJobOrderCommandValidator : AbstractValidator<CreateJobOrderCommand>
    {
        private readonly IJobOrderRepository _jobOrderRepository;
        public CreateJobOrderCommandValidator(IJobOrderRepository jobOrderRepository)
        {
            _jobOrderRepository = jobOrderRepository;

            RuleFor(p => p.JONo)
               .NotEmpty().WithMessage("{PropertyName} is required.")
               .NotNull().WithMessage("{PropertyName} should not be null.");


            RuleFor(p => p.JONo)
                .MustAsync(async (model, jono, _) =>
                    {
                        return await CheckExistingJobOrder(jono, model.ISNo); // assuming ISNo is another property
                    })
                .WithMessage("{PropertyName} already exists.")
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .NotNull().WithMessage("{PropertyName} should not be null.");
        }

        private async Task<bool> CheckExistingJobOrder(string jono, string isno)
        {
            var exists = await _jobOrderRepository.CheckJobOrderNo(jono, isno);
            return !exists; // Make sure JobOrder does not exist
        }
    }
}
