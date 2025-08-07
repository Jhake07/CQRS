using CQRS.Domain.Common;

namespace CQRS.Domain
{
    public class Product : BaseEntity
    {
        public required string ModelCode { get; set; }

        public required string Description { get; set; }

        public required string Brand { get; set; }

        public int DefaultJOQty { get; set; }

        public required string Components { get; set; }

        public int Accessories { get; set; }

        public required string Stats { get; set; }

    }
}
