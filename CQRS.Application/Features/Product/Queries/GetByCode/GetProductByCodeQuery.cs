using CQRS.Application.DTO;
using MediatR;

namespace CQRS.Application.Features.Product.Queries.GetByCode
{
    public record GetProductByCodeQuery(string Code) : IRequest<ProductDto>
    {
    }
}
