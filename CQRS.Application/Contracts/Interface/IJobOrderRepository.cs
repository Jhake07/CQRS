using CQRS.Domain;

namespace CQRS.Application.Contracts.Interface
{
    public interface IJobOrderRepository : IGenericRepository<JobOrder>
    {
        Task<bool> CheckJobOrderNo(string joNo, string isNo);
    }
}
