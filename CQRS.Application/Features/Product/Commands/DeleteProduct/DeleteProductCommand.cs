using CQRS.Application.Shared.Response;
using MediatR;

namespace CQRS.Application.Features.Product.Commands.DeleteProduct
{
    public class DeleteProductCommand : IRequest<CustomResultResponse>
    {
        public required int Id { get; set; }
    }

}
