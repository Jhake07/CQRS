using CQRS.Domain;

namespace CQRS.Application.Contracts.Interface
{
    public interface IMainSerialRepository : IGenericRepository<MainSerial>
    {
        Task BulkCreateAsync(IEnumerable<MainSerial> mainSerials, CancellationToken cancellationToken);
        Task UpdateContractNoAsync(string oldContractNo, string newContractNo);
        Task UpdateSerialJoNo(string mainSerial, string newJoNo);
        Task<string> GetJobOrderIdByMainSerialAsync(string mainserial);
    }
}
