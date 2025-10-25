namespace CQRS.Application.DTO
{
    public class JobOrderDto
    {
        public int Id { get; set; }
        public string JoNo { get; set; } = string.Empty;
        public string BatchSerial_ContractNo { get; set; } = string.Empty;
        public string Stats { get; set; } = string.Empty;
        public string OrderType { get; set; } = string.Empty;
        public int OrderQty { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Line { get; set; } = string.Empty;
        public int ProcessOrder { get; set; }
        public string IsNo { get; set; } = string.Empty;
        public required string CreatedBy { get; set; }
    }
}
