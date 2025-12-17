using System.ComponentModel.DataAnnotations;

namespace CQRS.Application.DTO.ScannedUnitsDto
{
    public class UpdateAccessoriesRequestDto
    {
        [Required]
        public string AccessoriesStatus { get; set; } = string.Empty;
    }
}
