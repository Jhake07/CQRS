namespace CQRS.Application.DTO
{
    public class BatchSerialDto
    {
        public int Id { get; set; }
        public string ContractNo { get; set; } = string.Empty;
        public string Customer { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string DocNo { get; set; } = string.Empty;
        public int BatchQty { get; set; }
        public int OrderQty { get; set; }
        public int DeliverQty { get; set; }
        public string Status { get; set; } = string.Empty;
        public required string SerialPrefix { get; set; }
        public required string StartSNo { get; set; }
        public required string EndSNo { get; set; }

        // Foreign Key / Navigation Properties       
        public required string Item_ModelCode { get; set; }
    }
}
