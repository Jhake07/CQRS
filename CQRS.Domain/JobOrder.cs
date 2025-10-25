using CQRS.Domain.Common;

namespace CQRS.Domain
{
    public class JobOrder : BaseEntity
    {
        public required string JoNo { get; set; }
        public required string BatchSerial_ContractNo { get; set; }
        public string Stats { get; set; } = string.Empty;
        public string OrderType { get; set; } = string.Empty;
        public int OrderQty { get; set; }
        public string Line { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int ProcessOrder { get; set; }
        public required string ISNo { get; set; }

    }
}
