using CQRS.Application.Models.Identity;
using FluentValidation;

namespace CQRS.Identity.Services
{
    public class AppAuthServiceValidatorUpdateRoleStatus : AbstractValidator<UpdateUserStatusRequest>
    {
        public AppAuthServiceValidatorUpdateRoleStatus()
        {
            RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required.")
            .Length(4, 30).WithMessage("Username must be between 4 and 30 characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.Role)
                .NotEmpty().WithMessage("Role is required.")
                .Length(4, 30).WithMessage("Role must be between 4 and 30 characters.");

            // Optional: You can validate IsActive as required, even though it’s a bool
            RuleFor(x => x.IsActive)
                .NotNull().WithMessage("Active status must be specified.");



        }
    }
}
