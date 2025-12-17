using CQRS.Application.Contracts.Interface;
using FluentValidation;

namespace CQRS.Application.Features.ScannedUnit.Commands.UpdateScannedUnit.UpdateTag
{
    public class UpdateTagCommandValidator : AbstractValidator<UpdateTagCommand>
    {
        protected readonly IScannedUnitsRepository _scannedUnitsRepository;
        public UpdateTagCommandValidator(IScannedUnitsRepository scannedUnitsRepository)
        {
            _scannedUnitsRepository = scannedUnitsRepository;

            // --- 1. Stateless Rules (Required Fields) ---
            RuleFor(p => p.MainSerial).NotEmpty().NotNull().WithMessage("The Main Serial number is required and cannot be empty.");
            RuleFor(p => p.NewTagNo).NotEmpty().WithMessage("New tag is required for tagging update.");

            // --- 2. Database-backed Rule (Existence Check) ---
            // This rule uses the new UnitExistsAsync method
            RuleFor(p => p.MainSerial)
                .MustAsync(BeAnExistingUnit)
                .WithMessage("The specified Main Serial was not found in the system.");
        }

        // Custom method uses the repository's efficient existence check
        private async Task<bool> BeAnExistingUnit(string mainSerial, CancellationToken cancellationToken)
        {
            // The method call is clean and direct
            return await _scannedUnitsRepository.UnitExistsAsync(mainSerial);
        }
    }
}
