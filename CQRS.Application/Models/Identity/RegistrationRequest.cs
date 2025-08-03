using System.ComponentModel.DataAnnotations;

namespace CQRS.Application.Models.Identity
{
    public class RegistrationRequest
    {
        [Required]
        public required string FirstName { get; set; }

        [Required]
        public required string LastName { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        [MinLength(6)]
        public required string UserName { get; set; }

        [Required]
        [MinLength(6)]
        public required string Password { get; set; }

        [Required]
        [MinLength(6)]
        public required string ConfirmPassword { get; set; }
    }
}
