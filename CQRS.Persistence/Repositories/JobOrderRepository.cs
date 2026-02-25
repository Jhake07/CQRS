using CQRS.Application.Contracts.Interface;
using CQRS.Application.Shared.Exceptions;
using CQRS.Domain;
using CQRS.Persistence.DBContext;
using Microsoft.EntityFrameworkCore;

namespace CQRS.Persistence.Repositories
{
    public class JobOrderRepository(CQRSPersistenceDbContext context) : GenericRepository<Domain.JobOrder>(context), IJobOrderRepository
    {
        public async Task<bool> CheckJobOrderNoAvailability(string joNo, string isNo, string lineNo)
        {
            var existingJobOrderNo = await _context.JobOrders
                .AsNoTracking()
                .AnyAsync(j => j.JoNo == joNo && j.ISNo == isNo && j.Line == lineNo);
            return existingJobOrderNo;
        }

        public async Task<bool> CheckJoStatus(int id)
        {
            var completeStatus = await _context.JobOrders
                .AsNoTracking()
                .AnyAsync(p => p.Id == id && p.Stats == "Completed");

            return completeStatus;
        }

        public async Task<bool> CheckOrderQty(string contractNo, int orderQty)
        {
            var batch = await _context.BatchSerials
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.ContractNo == contractNo);

            if (batch == null)
                return false; // or throw if contract must exist

            return orderQty <= batch.BatchQty;

        }

        public async Task<JobOrder> GetJobOrderById(int id)
        {
            // Retrieve the BatchSerial entity by its ID or throw an exception if not found
            var jobOrder = await _context.JobOrders
                .AsNoTracking() // Improves performance by not tracking the entity if updates are not needed
                .FirstOrDefaultAsync(x => x.Id == id)
                ?? throw new NotFoundException(nameof(JobOrder), id);

            return jobOrder;
        }

        public async Task AddProcessOrderQty(string joNo)
        {
            var jobOrderEntity = await _context.JobOrders
                .FirstOrDefaultAsync(j => j.JoNo == joNo);

            if (jobOrderEntity == null)
            {
                throw new JobOrderNotFoundException("JobOrder", joNo, "was not found.");
            }

            jobOrderEntity.ProcessOrder += 1;
            await _context.SaveChangesAsync();
        }

        public async Task<JobOrder> GetJobOrderCount(string joNo)
        {
            // Retrieve the BatchSerial entity by its ID or throw an exception if not found
            var jobOrder = await _context.JobOrders
                .AsNoTracking() // Improves performance by not tracking the entity if updates are not needed
                .FirstOrDefaultAsync(x => x.JoNo == joNo)
                ?? throw new NotFoundException(nameof(JobOrder), joNo);

            return jobOrder;
        }

        public async Task UpdateJoStatus(string joNo)
        {
            var jobOrderEntity = await _context.JobOrders
                .FirstOrDefaultAsync(j => j.JoNo == joNo);

            if (jobOrderEntity == null)
            {
                throw new JobOrderNotFoundException("JobOrder", joNo, "was not found.");
            }

            jobOrderEntity.Stats = "Completed";
            await _context.SaveChangesAsync();
        }
    }
}
