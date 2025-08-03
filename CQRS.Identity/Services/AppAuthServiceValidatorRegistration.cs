using CQRS.Application.Models.Identity;
using FluentValidation;

namespace CQRS.Identity.Services
{
    public class AppAuthServiceValidatorRegistration : AbstractValidator<RegistrationRequest>
    {
        public AppAuthServiceValidatorRegistration(RegistrationRequest request)
        {
            RuleFor(service => service).NotNull().WithMessage("AppAuthService cannot be null.");
            // Add more validation rules as needed
            RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(50);

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MaximumLength(50);

            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("Username is required.")
                .MinimumLength(4)
                .MaximumLength(30);

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters.");

            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.Password).WithMessage("Passwords do not match.");
        }
    }
}
