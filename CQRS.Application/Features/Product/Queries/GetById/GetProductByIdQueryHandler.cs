using AutoMapper;
using CQRS.Application.Contracts.Interface;
using CQRS.Application.DTO;
using CQRS.Application.Shared.Exceptions;
using MediatR;

namespace CQRS.Application.Features.Product.Queries.GetById
{
    public class GetProductByIdQueryHandler(IMapper mapper, IProductRepository productRepository) : IRequestHandler<GetProductByIdQuery, ProductDto>
    {
        private readonly IMapper _mapper = mapper;
        private readonly IProductRepository _productRepository = productRepository;
        public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            // Query the database
            var product = await _productRepository.GetProductById(request.Id) ?? throw new NotFoundException(nameof(Product), request.Id);
            // Convert data objects to DTO
            var data = _mapper.Map<ProductDto>(product);
            // Return DTO object
            return data;
        }
    }
}
