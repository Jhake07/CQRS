using CQRS.Application.Contracts.Interface;
using CQRS.Application.Shared.Exceptions;
using CQRS.Domain;
using CQRS.Persistence.DBContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace CQRS.Persistence.Repositories
{
    public class BatchSerialRepository(CQRSPersistenceDbContext context) : GenericRepository<BatchSerial>(context), IBatchSerialRepository
    {
        public async Task<List<BatchSerial>> GetBatchSerials()
        {
            var batchSerials = await _context.BatchSerials.Include(q => q.Item_ModelCode).ToListAsync();

            return batchSerials;
        }
        public async Task<BatchSerial> GetBatchSerialsByContractNo(string contractNo)
        {
            var batchSerials = await _context.BatchSerials
                .Where(x => x.ContractNo == contractNo)
                //.Include(q => q.Item_ModelCode)
                .FirstOrDefaultAsync() ?? throw new NotFoundException(nameof(BatchSerial), contractNo);

            return batchSerials;
        }
        public async Task<BatchSerial> GetBatchSerialsById(int id)
        {
            // Retrieve the BatchSerial entity by its ID or throw an exception if not found
            var batchSerial = await _context.BatchSerials
                .AsNoTracking() // Improves performance by not tracking the entity if updates are not needed
                .FirstOrDefaultAsync(x => x.Id == id)
                ?? throw new NotFoundException(nameof(BatchSerial), id);

            return batchSerial;
        }
        public async Task<bool> CheckBatchContractNo(string contractNo)
        {
            // Ensure the query properly filters records by ContractNo
            var isExistingContractNo = await _context.BatchSerials
                .AsNoTracking() // Add AsNoTracking for performance, since it's a read-only operation
                .AnyAsync(p => p.ContractNo == contractNo);

            return isExistingContractNo;
        }
        public async Task<bool> CheckMainSerialPrefix(string serialPrefix)
        {
            // Ensure the query properly filters records by ContractNo
            var isExistingSerialPrefix = await _context.BatchSerials
                .AsNoTracking() // Add AsNoTracking for performance, since it's a read-only operation
                .AnyAsync(p => p.SerialPrefix == serialPrefix);

            return isExistingSerialPrefix;
        }

        // Implementation for BeginTransactionAsync
        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }
    }
}
