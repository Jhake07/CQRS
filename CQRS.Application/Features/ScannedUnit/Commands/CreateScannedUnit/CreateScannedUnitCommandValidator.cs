using CQRS.Application.Contracts.Interface;
using CQRS.Application.Shared.Exceptions;
using FluentValidation;

namespace CQRS.Application.Features.ScannedUnit.Commands.CreateScannedUnit
{
    public class CreateScannedUnitCommandValidator : AbstractValidator<CreateScannedUnitCommand>
    {
        private readonly IScannedUnitsRepository _scannedUnitsRepository;
        public CreateScannedUnitCommandValidator(IScannedUnitsRepository scannedUnitsRepository)
        {
            _scannedUnitsRepository = scannedUnitsRepository;
            RuleFor(p => p.MainSerial)
              .NotEmpty().WithMessage("{PropertyName} is required.")
              .NotNull().WithMessage("{PropertyName} should not be null.")
              .MustAsync(CheckSerialNoAsync)
              .WithMessage("The specified Main Serial was not found in the system or is already assigned to a JO.");
        }
        private async Task<bool> CheckSerialNoAsync(string mainSerial, CancellationToken cancellationToken)
        {
            try
            {
                await _scannedUnitsRepository.CheckMainSerialAvailabilityAsync(mainSerial);
                return true;
            }
            catch (MainSerialNotFoundException)
            {
                return false;
            }
        }

    }
}
