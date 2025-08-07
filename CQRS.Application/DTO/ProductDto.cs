namespace CQRS.Application.DTO
{
    public class ProductDto
    {
        public string ModelCode { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Brand { get; set; } = string.Empty;

        public int DefaultJOQty { get; set; }

        public string Components { get; set; } = string.Empty;

        public int Accessories { get; set; }

        public string Stats { get; set; } = string.Empty;
    }
}
