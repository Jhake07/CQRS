using CQRS.Application.Shared.Response;
using MediatR;

namespace CQRS.Application.Features.Product.Commands.CreateProduct
{
    public class CreateProductCommand : IRequest<CustomResultResponse>
    {
        public required string ModelCode { get; set; }

        public required string Description { get; set; }

        public required string Brand { get; set; }

        public int DefaultJOQty { get; set; }

        public required string Components { get; set; }

        public int Accessories { get; set; }

        public required string Stats { get; set; }
        public required string CreatedBy { get; set; }
    }
}
