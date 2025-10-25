using CQRS.Domain;

namespace CQRS.Application.Contracts.Interface
{
    public interface IJobOrderRepository : IGenericRepository<JobOrder>
    {
        Task<bool> CheckJobOrderNo(string joNo, string isNo, string lineNo);
        Task<bool> CheckOrderQty(string contractNo, int orderQty);
        Task<bool> CheckJoStatus(int id);
        Task<JobOrder> GetJobOrderById(int id);
    }
}
