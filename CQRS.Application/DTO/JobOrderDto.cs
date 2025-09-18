namespace CQRS.Application.DTO
{
    public class JobOrderDto
    {
        public string JONo { get; set; } = string.Empty;
        public string BatchSerial_ContractNo { get; set; } = string.Empty;
        public string Stats { get; set; } = string.Empty;
        public string OrderType { get; set; } = string.Empty;
        public int OrderQty { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int Line1Qty { get; set; }
        public int ProcessOrderQtyLine1 { get; set; }
        public int Line2Qty { get; set; }
        public int ProcessOrderQtyLine2 { get; set; }
        public string ISNo { get; set; } = string.Empty;
        public int LeftQty { get; set; }
    }
}
