using CQRS.Application.Contracts.Interface;
using CQRS.Application.Shared.Exceptions;
using CQRS.Domain;
using CQRS.Persistence.DBContext;
using Microsoft.EntityFrameworkCore;

namespace CQRS.Persistence.Repositories
{
    public class ProductRepository(CQRSPersistenceDbContext context) : GenericRepository<Product>(context), IProductRepository
    {
        public async Task<bool> CheckProductCodeAsync(string code)
        {
            var existingCode = await _context.Products
                .AsNoTracking()
                .AnyAsync(p => p.ModelCode == code);

            return existingCode;
        }

        public Task<Product> GetProductByCodeAsync(string code)
        {
            var product = _context.Products
               .Where(x => x.ModelCode == code)
               .FirstOrDefaultAsync() ?? throw new NotFoundException(nameof(Product), code);

            return product;
        }

        public Task<Product> GetProductById(int id)
        {
            var product = _context.Products
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync() ?? throw new NotFoundException(nameof(Product), id);

            return product;
        }

        public async Task<List<Product>> GetProductsAsync()
        {
            var products = await _context.Products
                 .AsNoTracking() // Improves performance by not tracking the entity if updates are not needed
                 .ToListAsync();
            return products;
        }

        public Task<List<Product>> GetProductsByDescriptionAsync(string desc)
        {
            var product = _context.Products
               .Where(x => x.Description == desc)
               .ToListAsync();

            return product;
        }
    }
}
