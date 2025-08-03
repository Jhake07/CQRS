using System.ComponentModel.DataAnnotations;

namespace CQRS.Application.Models.Identity
{
    public class ResetPasswordRequest
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        [MinLength(6)]
        public required string UserName { get; set; }

    }
}
