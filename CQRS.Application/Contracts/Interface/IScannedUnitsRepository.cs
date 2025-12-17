using CQRS.Domain;

namespace CQRS.Application.Contracts.Interface
{
    public interface IScannedUnitsRepository : IGenericRepository<ScannedUnits>
    {
        Task UpdateUnitComponentsAsync(string mainserial, string motherboardSerial, string pcbiSerial, string powerSupplySerial);
        Task<bool> UnitExistsAsync(string mainSerial);
        Task UpdateUnitTagAsync(string mainserial, string newTagNo);
    }
}
