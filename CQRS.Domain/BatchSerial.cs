using CQRS.Domain.Common;

namespace CQRS.Domain
{
    public class BatchSerial : BaseEntity
    {
        public required string ContractNo { get; set; }
        public string Customer { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string DocNo { get; set; } = string.Empty;
        public int BatchQty { get; set; }
        public int OrderQty { get; set; }
        public int DeliverQty { get; set; }
        public int RemainingQty { get; set; }
        public string Status { get; set; } = string.Empty;
        public required string SerialPrefix { get; set; }
        public required string StartSNo { get; set; }
        public required string EndSNo { get; set; }

        // Foreign Key / Navigation Properties
        //public Item? Item { get; set; }
        public required string Item_ModelCode { get; set; }

    }
}
