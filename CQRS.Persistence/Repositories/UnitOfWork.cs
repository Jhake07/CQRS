using CQRS.Application.Contracts.Interface;
using CQRS.Persistence.DBContext;

namespace CQRS.Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly CQRSPersistenceDbContext _context;

        public UnitOfWork(CQRSPersistenceDbContext context)
        {
            _context = context;
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }
    }

}
