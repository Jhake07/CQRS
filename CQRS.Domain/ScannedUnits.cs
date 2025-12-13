using CQRS.Domain.Common;

namespace CQRS.Domain
{
    public class ScannedUnits : BaseEntity
    {
        public string ContractNo { get; set; } = string.Empty;
        public string JoNo { get; set; } = string.Empty;
        public string MainSerial { get; set; } = string.Empty;
        public string? TagNo { get; set; } = string.Empty;
        public string? Accessories { get; set; } = string.Empty;
        public string? Line { get; set; } = string.Empty;
        public string? Motherboard { get; set; } = string.Empty;
        public string? PCBI { get; set; } = string.Empty;
        public string? PowerSupply { get; set; } = string.Empty;
        public string? ScanBy { get; set; } = string.Empty;
        public DateTime? ScanDate { get; set; }
    }
}
