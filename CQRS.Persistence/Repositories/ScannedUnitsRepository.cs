using CQRS.Application.Contracts.Interface;
using CQRS.Application.Shared.Exceptions;
using CQRS.Domain;
using CQRS.Persistence.DBContext;
using Microsoft.EntityFrameworkCore;

namespace CQRS.Persistence.Repositories
{
    public class ScannedUnitsRepository(CQRSPersistenceDbContext context) : GenericRepository<ScannedUnits>(context), IScannedUnitsRepository
    {

        // --- Update Accessories ---
        public async Task UpdateUnitAccessoriesAsync(string mainserial, string accessoriesSerial)
        {
            await _context.Set<ScannedUnits>()
                .Where(u => u.MainSerial == mainserial)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(u => u.Accessories, accessoriesSerial)
                    .SetProperty(u => u.ModifiedDate, DateTime.UtcNow)
                );
        }

        // --- Update Tag ---
        public async Task UpdateUnitTagAsync(string mainserial, string newTagNo)
        {
            await _context.Set<ScannedUnits>()
                .Where(u => u.MainSerial == mainserial)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(u => u.TagNo, newTagNo)
                    .SetProperty(u => u.ModifiedDate, DateTime.UtcNow)
                );
        }

        // --- Update Components ---
        public async Task UpdateUnitComponentsAsync(string mainserial, string motherboardSerial, string pcbiSerial, string powerSupplySerial)
        {
            await _context.Set<ScannedUnits>()
                .Where(u => u.MainSerial == mainserial)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(u => u.Motherboard, motherboardSerial)
                    .SetProperty(u => u.PCBI, pcbiSerial)
                    .SetProperty(u => u.PowerSupply, powerSupplySerial)
                    .SetProperty(u => u.ScanDate, DateTime.UtcNow)
                );
        }

        // -- Scan Unit Existence Check --
        public async Task<bool> UnitExistsAsync(string mainSerial)
        {
            // Use EF Core's AnyAsync for an optimized SQL EXISTS query
            return await _context.ScannedUnits
                .AnyAsync(u => u.MainSerial == mainSerial);
        }

        // --- Main Serial Availability Validation ---
        public async Task<bool> CheckMainSerialAvailabilityAsync(string mainSerial)
        {
            var unitDetails = await _context.MainSerials
                .AsNoTracking()
                .AnyAsync(u => u.SerialNo == mainSerial && (u.JoNo == null || u.JoNo == ""));

            return unitDetails;

        }

        // --- Get Batch Contract Number and Job Order Number for a Main Serial ---
        public async Task<MainSerial> GetMainSerialContractNo(string mainSerial)
        {
            var unitDetails = await _context.MainSerials
                .FirstOrDefaultAsync(u => u.SerialNo == mainSerial);

            if (unitDetails == null)
            {
                throw new MainSerialNotFoundException("MainSerial", mainSerial, "is not found");
            }

            return unitDetails;
        }

        public async Task<string> GetJobOrderNumberAsync(string batchContractNumber)
        {
            var jobOrderEntity = await _context.JobOrders
                .FirstOrDefaultAsync(j => j.BatchSerial_ContractNo == batchContractNumber && j.Stats == "In Progress");

            if (jobOrderEntity == null)
            {
                throw new JobOrderNotFoundException("Contract No", batchContractNumber, " job order was not found.");
            }

            return jobOrderEntity.JoNo;
        }

        public async Task UpdateMainSerialAsync(string mainSerial, string jobOrderNumber)
        {
            var mainSerialEntity = await _context.MainSerials
                .FirstOrDefaultAsync(u => u.SerialNo == mainSerial);

            if (mainSerialEntity == null)
            {
                throw new MainSerialNotFoundException("MainSerial", mainSerial, "is either not available or already assigned to a JO.");
            }

            mainSerialEntity.JoNo = jobOrderNumber;
            await _context.SaveChangesAsync();
        }

    }
}
