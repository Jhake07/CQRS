using CQRS.Domain;

namespace CQRS.Application.Contracts.Interface
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<List<Product>> GetProductsAsync();
        Task<Product> GetProductById(int id);
        Task<Product> GetProductByCodeAsync(string code);
        Task<List<Product>> GetProductsByDescriptionAsync(string desc);
        Task<bool> CheckProductCodeAsync(string code);
        Task<Product?> FindProductByCodeAsync(string code);
    }
}
