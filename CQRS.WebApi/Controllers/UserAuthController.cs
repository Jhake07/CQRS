using CQRS.Application.Contracts.Identity;
using CQRS.Application.DTO;
using CQRS.Application.Models.Identity;
using CQRS.Application.Shared.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CQRS.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserAuthController : ControllerBase
    {
        private readonly IAppAuthServiceRepository _appAuthServiceRepository;
        private readonly IAppUserServiceRepository _appUserServiceRepository;

        public UserAuthController(IAppAuthServiceRepository appAuthServiceRepository, IAppUserServiceRepository appUserServiceRepository)
        {
            _appAuthServiceRepository = appAuthServiceRepository;
            _appUserServiceRepository = appUserServiceRepository;
        }

        // GET: api/<BatchSerialController>
        [HttpGet]
        public async Task<List<AppUserDetailsDto>> Get()
        {
            return await _appUserServiceRepository.GetUsers();

        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login(AuthRequest request)
        {
            return Ok(await _appAuthServiceRepository.Login(request));
        }

        [HttpPost("register")]
        public async Task<ActionResult<RegistrationResponse>> Register(RegistrationRequest request, CancellationToken cancellationToken)
        {
            return Ok(await _appAuthServiceRepository.Register(request, cancellationToken));
        }

        [HttpPut("update-status-role")]
        public async Task<ActionResult<CustomResultResponse>> UpdateStatusAndRole([FromBody] UpdateUserStatusRequest request, CancellationToken cancellationToken)
        {
            return Ok(await _appAuthServiceRepository.UpdateUserStatus(request));
        }

        [HttpPut("change-password")]
        [AllowAnonymous] // Only for testing; switch to [Authorize] before production
        public async Task<ActionResult<CustomResultResponse>> ChangeUserPassword([FromBody] UpdateUserCredentialsRequest request, CancellationToken cancellationToken)
        {

            // Bypass identity check if testing anonymously
#if !DEBUG
                if (User?.Identity?.Name == null ||
                    !string.Equals(User.Identity.Name, request.Username, StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(new CustomResultResponse
                    {
                        IsSuccess = false,
                        Message = "You are not authorized to update another user's credentials.",
                        Id = request.Username
                    });
                }
#endif          

            if (User?.Identity?.Name == null ||
                   !string.Equals(User.Identity.Name, request.Username, StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(new CustomResultResponse
                {
                    IsSuccess = false,
                    Message = "You are not authorized to update another user's credentials.",
                    Id = request.Username
                });
            }
            var result = await _appAuthServiceRepository.UpdateUserCredentials(request);
            return StatusCode(result.IsSuccess ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);
        }

        [HttpPut("reset-password")]
        [AllowAnonymous] // Only for testing; switch to [Authorize] before production       
        public async Task<ActionResult<CustomResultResponse>> ResetUserCredentials([FromBody] ResetPasswordRequest request, CancellationToken cancellationToken)
        {
            return Ok(await _appAuthServiceRepository.ResetUserCredentials(request));

        }
    }
}
