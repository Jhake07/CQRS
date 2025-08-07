using CQRS.Application.DTO;
using MediatR;

namespace CQRS.Application.Features.Product.Queries.GetById
{
    public record GetProductByIdQuery(int Id) : IRequest<ProductDto>
    {
    }
}
