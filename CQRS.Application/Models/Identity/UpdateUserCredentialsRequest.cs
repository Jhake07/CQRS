using System.ComponentModel.DataAnnotations;

namespace CQRS.Application.Models.Identity
{
    public class UpdateUserCredentialsRequest
    {
        [Required]
        public required string Username { get; set; }
        [Required]
        public required string Email { get; set; }
        [Required]
        public required string CurrentPassword { get; set; }
        [Required]
        public required string NewPassword { get; set; }
        [Required]
        public required string ConfirmPassword { get; set; }

    }
}
