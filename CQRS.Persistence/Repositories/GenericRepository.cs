using CQRS.Application.Contracts.Interface;
using CQRS.Application.Shared.Exceptions;
using CQRS.Domain.Common;
using CQRS.Persistence.DBContext;
using Microsoft.EntityFrameworkCore;

namespace CQRS.Persistence.Repositories
{
    public class GenericRepository<T>(CQRSPersistenceDbContext context) : IGenericRepository<T> where T : BaseEntity
    {
        protected readonly CQRSPersistenceDbContext _context = context;

        public async Task CreateAsync(T entity)
        {
            await _context.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(T entity)
        {
            _context.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<IReadOnlyList<T>> GetAsync()
        {
            return await _context.Set<T>().AsNoTracking().ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            var entity = await _context.Set<T>()
                .AsNoTracking()
                .FirstOrDefaultAsync(q => q.Id == id);

            // If entity is null, throw a NotFoundException
            if (entity == null)
            {
                throw new NotFoundException(typeof(T).Name, id);
            }

            return entity;
        }

        public async Task UpdateAsync(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity), "The entity to update cannot be null.");
            }

            // Retrieve the existing entity from the database
            var existingEntity = await _context.Set<T>().FindAsync(entity.Id);
            if (existingEntity == null)
            {
                throw new KeyNotFoundException($"Entity with Id {entity.Id} was not found.");
            }

            // Update the fields of the existing entity
            _context.Entry(existingEntity).CurrentValues.SetValues(entity);

            // Save changes to the database
            await _context.SaveChangesAsync();
        }
    }
}
