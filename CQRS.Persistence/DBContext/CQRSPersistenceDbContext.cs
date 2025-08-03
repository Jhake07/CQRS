using CQRS.Domain;
using CQRS.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace CQRS.Persistence.DBContext
{
    public class CQRSPersistenceDbContext(DbContextOptions<CQRSPersistenceDbContext> options) : DbContext(options)
    {
        public DbSet<BatchSerial> BatchSerials { get; set; }
        public DbSet<MainSerial> MainSerials { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CQRSPersistenceDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in base.ChangeTracker.Entries<BaseEntity>()
                .Where(q => q.State == EntityState.Added || q.State == EntityState.Modified))
            {
                entry.Entity.ModifiedDate = DateTime.UtcNow;

                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedDate = DateTime.UtcNow;
                }
            }
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
