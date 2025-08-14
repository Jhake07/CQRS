using AutoMapper;
using CQRS.Application.DTO;
using CQRS.Application.Features.Product.Commands.CreateProduct;
using CQRS.Application.Features.Product.Commands.DeleteProduct;
using CQRS.Application.Features.Product.Commands.UpdateProduct;
using CQRS.Domain;


namespace CQRS.Application.MappingProfiles
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            // Mapping from CreateProductCommand to Product entity
            // This will map the properties of CreateProductCommand to the corresponding properties in the Product entity
            CreateMap<CreateProductCommand, Product>();
            CreateMap<UpdateProductCommand, Product>();
            CreateMap<DeleteProductCommand, Product>();
            CreateMap<ProductDto, Product>().ReverseMap();
        }
    }
}
