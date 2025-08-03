using System.ComponentModel.DataAnnotations;

namespace CQRS.Application.Models.Identity
{
    public class UpdateUserStatusRequest
    {
        [Required]
        public required string Username { get; set; }
        [Required]
        public required string Email { get; set; }
        [Required]
        public required bool IsActive { get; set; }
        [Required]
        public required string Role { get; set; }
    }
}
