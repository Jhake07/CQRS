using AutoMapper;
using CQRS.Application.Contracts.Interface;
using CQRS.Application.DTO;
using CQRS.Application.Shared.Exceptions;
using MediatR;

namespace CQRS.Application.Features.Product.Queries.GetByCode
{
    public class GetProductByCodeQueryHandler(IMapper mapper, IProductRepository productRepository) : IRequestHandler<GetProductByCodeQuery, ProductDto>
    {
        private readonly IMapper _mapper = mapper;
        private readonly IProductRepository _productRepository = productRepository;

        public async Task<ProductDto> Handle(GetProductByCodeQuery request, CancellationToken cancellationToken)
        {
            // Query the database
            var product = await _productRepository.GetProductByCodeAsync(request.Code) ?? throw new NotFoundException(nameof(Product), request.Code);

            // Convert data objects to DTO
            var data = _mapper.Map<ProductDto>(product);

            // Return DTO object
            return data;
        }
    }
}
