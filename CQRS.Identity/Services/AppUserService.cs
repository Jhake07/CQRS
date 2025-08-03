using CQRS.Application.Contracts.Identity;
using CQRS.Application.DTO;
using CQRS.Identity.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace CQRS.Identity.Services
{
    public class AppUserService : IAppUserServiceRepository
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IHttpContextAccessor _contextAccessor;
        public AppUserService(UserManager<AppUser> userManager, IHttpContextAccessor contextAccessor)
        {
            _userManager = userManager;
            _contextAccessor = contextAccessor;
        }

        public string UserId { get => _contextAccessor.HttpContext?.User?.FindFirstValue("uid"); }

        public async Task<AppUserDetailsDto> GetUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return null;

            var roles = await _userManager.GetRolesAsync(user);

            return MapToAppUserDto(user, roles);

        }

        public async Task<List<AppUserDetailsDto>> GetUsers()
        {
            var users = _userManager.Users.ToList();

            var userDtoList = new List<AppUserDetailsDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userDtoList.Add(MapToAppUserDto(user, roles));
            }

            return userDtoList;

        }
        private static AppUserDetailsDto MapToAppUserDto(AppUser user, IList<string> roles)
        {
            return new AppUserDetailsDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                IsActive = user.IsActive,
                CreatedDate = user.CreatedDate,
                Username = user.UserName,
                Role = roles?.FirstOrDefault() ?? string.Empty
            };
        }
    }
}
