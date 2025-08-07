using AutoMapper;
using CQRS.Application.Contracts.Interface;
using CQRS.Application.Contracts.Logging;
using CQRS.Application.DTO;
using MediatR;

namespace CQRS.Application.Features.Product.Queries.GetAll
{
    public class GetProductQueryHandler(IMapper mapper,
        IProductRepository productRepository,
        IAppLogger<GetProductQueryHandler> logger) :
        IRequestHandler<GetProductQuery, List<ProductDto>>
    {
        private readonly IMapper _mapper = mapper;
        private readonly IProductRepository _productRepository = productRepository;
        private readonly IAppLogger<GetProductQueryHandler> _logger = logger;

        public async Task<List<ProductDto>> Handle(GetProductQuery request, CancellationToken cancellationToken)
        {
            // Query the database
            var products = await _productRepository.GetProductsAsync();

            // Convert data object to DTO
            var data = _mapper.Map<List<ProductDto>>(products);

            // Return the list of DTO object
            _logger.LogInformation("Product retrieve successfully.");
            return data;
        }
    }
}
