using CQRS.Application.DTO;
using MediatR;

namespace CQRS.Application.Features.Product.Queries.GetAll
{
    public record GetProductQuery : IRequest<List<ProductDto>>
    {
    }
}
