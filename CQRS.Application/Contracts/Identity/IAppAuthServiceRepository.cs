using CQRS.Application.Models.Identity;
using CQRS.Application.Shared.Response;

namespace CQRS.Application.Contracts.Identity
{
    public interface IAppAuthServiceRepository
    {
        Task<AuthResponse> Login(AuthRequest request);
        Task<IdentityResultResponse> Register(RegistrationRequest request, CancellationToken cancellationToken);
        Task<CustomResultResponse> UpdateUserStatus(UpdateUserStatusRequest request);
        Task<CustomResultResponse> UpdateUserCredentials(UpdateUserCredentialsRequest request);
        Task<CustomResultResponse> ResetUserCredentials(ResetPasswordRequest request);
    }
}
