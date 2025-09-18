using CQRS.Application.Contracts.Interface;
using CQRS.Persistence.DBContext;
using Microsoft.EntityFrameworkCore;

namespace CQRS.Persistence.Repositories
{
    public class JobOrderRepository(CQRSPersistenceDbContext context) : GenericRepository<Domain.JobOrder>(context), IJobOrderRepository
    {
        public async Task<bool> CheckJobOrderNo(string joNo, string isNo)
        {
            var existingJobOrderNo = await _context.JobOrders
                .AsNoTracking()
                .AnyAsync(j => j.JONo == joNo || j.ISNo == isNo);
            return existingJobOrderNo;
        }
    }
}
