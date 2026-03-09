using CQRS.Application.Contracts.Interface;
using FluentValidation;
namespace CQRS.Application.Features.ScannedUnit.Commands.CreateScannedUnit
{
    public class CreateScannedUnitCommandValidator : AbstractValidator<CreateScannedUnitCommand>
    {
        public CreateScannedUnitCommandValidator(IScannedUnitsRepository scannedUnitsRepository)
        {
            RuleFor(x => x.MainSerial)
                .NotEmpty().WithMessage("MainSerial is required.")
                .MustAsync(async (serial, ct) =>
                    await scannedUnitsRepository.CheckMainSerialAvailabilityAsync(serial))
                .WithMessage("The specified main serial was not found or is already assigned to a JO.");
        }
    }
}