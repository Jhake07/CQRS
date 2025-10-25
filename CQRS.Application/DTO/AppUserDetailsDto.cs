namespace CQRS.Application.DTO
{
    public class AppUserDetailsDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? ModifiedDate { get; set; } = DateTime.UtcNow;
        public string? Username { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public required string CreatedBy { get; set; }
    }
}
