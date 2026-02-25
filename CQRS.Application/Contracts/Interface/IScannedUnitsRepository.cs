using CQRS.Domain;

namespace CQRS.Application.Contracts.Interface
{
    public interface IScannedUnitsRepository : IGenericRepository<ScannedUnits>
    {
        Task UpdateUnitComponentsAsync(string mainserial, string motherboardSerial, string pcbiSerial, string powerSupplySerial);
        Task<bool> UnitExistsAsync(string mainSerial);
        Task UpdateUnitTagAsync(string mainserial, string newTagNo);
        Task UpdateUnitAccessoriesAsync(string mainserial, string accessoriesSerial);
        Task<bool> CheckMainSerialAvailabilityAsync(string mainSerial);
        Task<MainSerial> GetMainSerialContractNo(string mainSerial);
        Task<string> GetJobOrderNumberAsync(string batchContractNumber);
    }
}
