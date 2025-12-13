using System.ComponentModel.DataAnnotations;

namespace CQRS.Application.DTO.ScannedUnitsDto
{
    public class UpdateComponentsRequestDto
    {
        [Required]
        public string MotherboardSerial { get; set; } = string.Empty;

        [Required]
        public string PcbiSerial { get; set; } = string.Empty;

        [Required]
        public string PowerSupplySerial { get; set; } = string.Empty;
    }
}
