using CQRS.Application.Contracts.Interface;
using CQRS.Application.Shared.Exceptions;
using CQRS.Domain;
using CQRS.Persistence.DBContext;
using Microsoft.EntityFrameworkCore;

namespace CQRS.Persistence.Repositories
{
    public class MainSerialRepository(CQRSPersistenceDbContext context) : GenericRepository<MainSerial>(context), IMainSerialRepository
    {
        public async Task BulkCreateAsync(IEnumerable<MainSerial> mainSerials, CancellationToken cancellationToken)
        {
            try
            {
                // Use EF Core's `AddRangeAsync` to add multiple entities in a single operation
                await _context.Set<Domain.MainSerial>().AddRangeAsync(mainSerials, cancellationToken);

                // Save changes to persist the records in the database
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                // Log error and rethrow or handle appropriately
                Console.WriteLine($"An error occurred during bulk insert: {ex.Message}");
                throw;
            }

        }
        public async Task UpdateContractNoAsync(string oldContractNo, string newContractNo)
        {
            // Retrieve all MainSerial records with the old contract number
            var mainSerialRecords = await _context.MainSerials
                .Where(m => m.BatchSerial_ContractNo == oldContractNo)
                .ToListAsync();

            // Check if any records were found
            if (!mainSerialRecords.Any())
            {
                return;
            }

            // Update all records with the new contract number
            mainSerialRecords.ForEach(m => m.BatchSerial_ContractNo = newContractNo);

            // Save all changes to the database
            await _context.SaveChangesAsync();
        }

        public async Task UpdateSerialJoNo(string mainSerial, string newJoNo)
        {
            await _context.Set<MainSerial>()
                .Where(u => u.SerialNo == mainSerial)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(u => u.JoNo, newJoNo)
                    .SetProperty(u => u.ModifiedDate, DateTime.UtcNow)
                );
        }

        public async Task<string> GetJobOrderIdByMainSerialAsync(string mainserial)
        {
            var unitDetails = await _context.MainSerials
               .FirstOrDefaultAsync(u => u.SerialNo == mainserial);

            if (unitDetails == null)
            {
                throw new MainSerialNotFoundException("MainSerial", mainserial, "is not found");
            }

            return unitDetails.JoNo;
        }
    }
}
