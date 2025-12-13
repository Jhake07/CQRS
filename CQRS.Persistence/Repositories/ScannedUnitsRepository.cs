using CQRS.Application.Contracts.Interface;
using CQRS.Domain;
using CQRS.Persistence.DBContext;
using Microsoft.EntityFrameworkCore;

namespace CQRS.Persistence.Repositories
{
    public class ScannedUnitsRepository(CQRSPersistenceDbContext context) : GenericRepository<ScannedUnits>(context), IScannedUnitsRepository
    {
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

        public async Task<bool> UnitExistsAsync(string mainSerial)
        {
            // Use EF Core's AnyAsync for an optimized SQL EXISTS query
            return await _context.ScannedUnits
                .AnyAsync(u => u.MainSerial == mainSerial);
        }
    }
}
