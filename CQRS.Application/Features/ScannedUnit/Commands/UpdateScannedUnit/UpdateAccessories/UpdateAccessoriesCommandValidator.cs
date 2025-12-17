using CQRS.Application.Contracts.Interface;
using FluentValidation;

namespace CQRS.Application.Features.ScannedUnit.Commands.UpdateScannedUnit.UpdateAccessories
{
    partial class UpdateAccessoriesCommandValidator : AbstractValidator<UpdateAccessoriesCommand>
    {
        protected readonly IScannedUnitsRepository _scannedUnitsRepository;
        public UpdateAccessoriesCommandValidator(IScannedUnitsRepository scannedUnitsRepository)
        {
            _scannedUnitsRepository = scannedUnitsRepository;

            // --- 1. Stateless Rules (Required Fields) ---
            RuleFor(p => p.mainSerial).NotEmpty().NotNull().WithMessage("The Main Serial number is required and cannot be empty.");
            RuleFor(p => p.accessoriesSerial).NotEmpty().WithMessage("Accessories serial is required for this update.");

            // --- 2. Database-backed Rule (Existence Check) ---
            // This rule uses the new UnitExistsAsync method
            RuleFor(p => p.mainSerial)
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
