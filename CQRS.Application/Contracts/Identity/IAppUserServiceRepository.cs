using CQRS.Application.DTO;

namespace CQRS.Application.Contracts.Identity
{
    public interface IAppUserServiceRepository
    {
        Task<List<AppUserDetailsDto>> GetUsers();
        Task<AppUserDetailsDto> GetUser(string userId);
        public string UserId { get; }
    }
}
