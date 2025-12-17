using System.ComponentModel.DataAnnotations;

namespace CQRS.Application.DTO.ScannedUnitsDto
{
    public class UpdateTagRequestDto
    {
        // TagNo is the specific piece of data being updated
        [Required]
        public string NewTagNo { get; set; } = string.Empty;

        // Optional: Include ScanBy if you want to track who performed the action
        // public string ScanBy { get; set; } = string.Empty; 
    }
}
