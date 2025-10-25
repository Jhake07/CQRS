using CQRS.Application.Contracts.Identity;
using CQRS.Application.Models.Identity;
using CQRS.Application.Shared.Exceptions;
using CQRS.Application.Shared.Response;
using CQRS.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CQRS.Identity.Services
{
    public class AppAuthService(UserManager<AppUser> userManager, IOptions<JwtSettings> jwtSettings, SignInManager<AppUser> signInManager,
      ILogger<AppAuthService> logger) : IAppAuthServiceRepository
    {
        private readonly UserManager<AppUser> _userManager = userManager;
        private readonly SignInManager<AppUser> _signInManager = signInManager;
        private readonly JwtSettings _jwtSettings = jwtSettings.Value;
        private readonly ILogger<AppAuthService> _logger = logger; // Add logger

        public async Task<AuthResponse> Login([FromBody] AuthRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
            {
                return new AuthResponse
                {
                    IsSuccess = false,
                    Message = $"User with Email: {request.Email} was not found."
                };
            }

            if (!user.IsActive)
            {

                return new AuthResponse
                {
                    IsSuccess = false,
                    Message = $"User with Email: {request.Email} is not active."
                };
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);

            if (!result.Succeeded)
            {
                return new AuthResponse
                {
                    IsSuccess = false,
                    Message = $"Credentials for '{request.Email} aren't valid'.",
                    Id = request.Email // Ensure the ID is converted to string if necessary
                };
            }

            JwtSecurityToken jwtSecurityToken = await GenerateToken(user);

            var response = new AuthResponse
            {
                //Id = user.Id,
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                Email = user.Email,
                UserName = user.UserName,
                IsSuccess = true,
                Message = "Login successful"
            };


            return response;
        }
        public async Task<IdentityResultResponse> Register([FromBody] RegistrationRequest request, CancellationToken cancellationToken)
        {
            try
            {
                // Validate data
                var validator = new AppAuthServiceValidatorRegistration(request);
                var validatorResult = await validator.ValidateAsync(request, cancellationToken);

                if (validatorResult.Errors.Count != 0)
                {
                    _logger.LogWarning("Validation failed when creating the account: {Errors}", validatorResult.Errors);
                    throw new BadRequestException("Validation failed", validatorResult);
                }

                var existingEmail = await _userManager.FindByEmailAsync(request.Email);
                if (existingEmail != null)
                {
                    var failure = new FluentValidation.Results.ValidationResult(
                    [
                        new FluentValidation.Results.ValidationFailure(nameof(request.Email), "Email is already in use.")
                    ]);

                    throw new BadRequestException("Email already exists", failure);
                }

                // Generate default password                
                var defaultPassword = GenerateDefaultPassword(request.FirstName, request.LastName, DateTime.Now.Year);

                var user = new AppUser
                {
                    Email = request.Email,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    UserName = request.UserName,
                    EmailConfirmed = true,
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow,
                    ModifiedDate = null,
                    CreatedBy = request.CreatedBy

                };

                var result = await _userManager.CreateAsync(user, defaultPassword);
                _logger.LogInformation("Created user: Username = {Username}, Id = {Id}", user.UserName, user.Id);

                if (!result.Succeeded)
                {
                    var identityFailures = result.Errors
                        .Select(e => new FluentValidation.Results.ValidationFailure("", e.Description))
                        .ToList();

                    var validationResult = new FluentValidation.Results.ValidationResult(identityFailures);

                    _logger.LogWarning("Identity validation failed for {Email}: {Errors}", request.Email, identityFailures);

                    throw new BadRequestException("Identity validation failed", validationResult);
                }

                user = await _userManager.FindByEmailAsync(user.Email);

                var roleResult = await _userManager.AddToRoleAsync(user, "User");

                if (!roleResult.Succeeded)
                {
                    var errorList = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                    _logger.LogError("Failed to assign role to {Email}: {Errors}", user.Email, errorList);
                    throw new Exception($"Could not assign role: {errorList}");
                }

                return new IdentityResultResponse
                {
                    IsSuccess = true,
                    Message = $"User registered successfully. Username: {request.UserName}",
                    Email = user.Email,
                    Id = user.Id
                };
            }
            catch (BadRequestException ex)
            {
                _logger.LogError(ex, "Validation error occurred for User: {Email}", request.Email);

                return new IdentityResultResponse
                {
                    IsSuccess = false,
                    Message = "Authentication Service Validation failed.",
                    ValidationErrors = ex.ValidationErrors,
                    Id = null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering user");

                throw new ApplicationException("An unexpected error occurred while registering the user. Please try again.");
            }
        }
        public async Task<CustomResultResponse> UpdateUserStatus([FromBody] UpdateUserStatusRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Email))
                {
                    return Fail("Username and Email must be provided.");
                }

                var user = await GetVerifiedUserAsync(request.Username, request.Email);
                if (user == null)
                {
                    return Fail("The provided username and email do not match the same account.");
                }

                // Validate role and status update
                var validator = new AppAuthServiceValidatorUpdateRoleStatus();
                var validationResult = await validator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    LogValidation("Updating user role and status", validationResult.Errors);
                    throw new BadRequestException("Validation failed for role/status update", validationResult);
                }

                // Apply updates
                if (!string.IsNullOrWhiteSpace(request.Role))
                {
                    await UpdateUserRoleAsync(user, request.Role);
                }

                user.IsActive = request.IsActive;
                user.ModifiedDate = DateTime.UtcNow;

                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    HandleFailure("User status update failed", updateResult.Errors, user.Id);
                }

                _logger.LogInformation("User status/role updated: {UserId}", user.Id);

                return new CustomResultResponse
                {
                    IsSuccess = true,
                    Message = "User status and role updated successfully.",
                    Id = user.Id
                };
            }
            catch (BadRequestException ex)
            {
                _logger.LogError(ex, "Validation error while updating user status: {Username}", request.Username);
                return new CustomResultResponse
                {
                    IsSuccess = false,
                    Message = "User status/role update failed.",
                    ValidationErrors = ex.ValidationErrors,
                    Id = request.Username
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during status update: {Username}", request.Username);
                throw new ApplicationException("An unexpected error occurred while updating the user status.");
            }
        }
        public async Task<CustomResultResponse> UpdateUserCredentials([FromBody] UpdateUserCredentialsRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Email))
                {
                    return Fail("Username and Email must be provided.");
                }

                var user = await GetVerifiedUserAsync(request.Username, request.Email);
                if (user == null)
                {
                    return Fail("The provided username and email do not match any account.");
                }

                // Validate new password
                var validator = new AppAuthServiceValidatorUpdatePassword();
                var validationResult = await validator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    LogValidation("Password change", validationResult.Errors);
                    throw new BadRequestException("Password validation failed.", validationResult);
                }

                // Confirm old password
                var isCurrentPasswordValid = await _userManager.CheckPasswordAsync(user, request.CurrentPassword);
                if (!isCurrentPasswordValid)
                {
                    return Fail("Current password is incorrect.");
                }

                // Update password
                var changeResult = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
                if (!changeResult.Succeeded)
                {
                    HandleFailure("Password update failed", changeResult.Errors, user.Id);
                }

                // Update timestamp
                user.ModifiedDate = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);

                _logger.LogInformation("User password changed: {UserId}", user.Id);

                return new CustomResultResponse
                {
                    IsSuccess = true,
                    Message = "Password updated successfully.",
                    Id = user.Id
                };
            }
            catch (BadRequestException ex)
            {
                _logger.LogError(ex, "Validation error: {Username}", request.Username);
                return new CustomResultResponse
                {
                    IsSuccess = false,
                    Message = "Password update failed due to validation error.",
                    ValidationErrors = ex.ValidationErrors,
                    Id = request.Username
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error: {Username}", request.Username);
                throw new ApplicationException("An unexpected error occurred during password update.");
            }
        }
        public async Task<CustomResultResponse> ResetUserCredentials([FromBody] ResetPasswordRequest request)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.UserName))
            {
                return new CustomResultResponse
                {
                    IsSuccess = false,
                    Message = "Email and Username are required.",
                    Id = request.UserName
                };
            }

            // Find user by email
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null || !string.Equals(user.UserName, request.UserName, StringComparison.OrdinalIgnoreCase))
            {
                return new CustomResultResponse
                {
                    IsSuccess = false,
                    Message = $"User with email '{request.Email}' and username '{request.UserName}' not found.",
                    Id = request.UserName
                };
            }

            if (!user.IsActive)
            {
                return new CustomResultResponse
                {
                    IsSuccess = false,
                    Message = $"User '{request.UserName}' is inactive and cannot be reset.",
                    Id = user.Id
                };
            }

            // Generate temporary default password 9format: Username + @ + current year)
            var tempPassword = GenerateDefaultPassword(user.FirstName, user.LastName, DateTime.Now.Year);

            // Use Identity to reset password
            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, resetToken, tempPassword);

            if (!result.Succeeded)
            {
                return new CustomResultResponse
                {
                    IsSuccess = false,
                    Message = $"Password reset failed: {string.Join("; ", result.Errors.Select(e => e.Description))}",
                    Id = user.Id
                };
            }

            // Optionally email or log the temporary password
            //await _emailService.SendPasswordResetNotification(user.Email, user.UserName, tempPassword);

            return new CustomResultResponse
            {
                IsSuccess = true,
                Message = $"Password successfully reset for '{user.UserName}'.",
                Id = user.Id
            };
        }

        // Additional methods (can be added here as needed)
        private async Task<JwtSecurityToken> GenerateToken(AppUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? "User"; // fallback if empty
            var roleClaims = roles.Select(q => new Claim(ClaimTypes.Role, q)).ToList();

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("firstName", user.FirstName ?? ""),
                //new Claim("isActive", user.IsActive.ToString()),
                new Claim("id", user.Id),

                new Claim(ClaimTypes.Name, user.UserName),        // Enables User.Identity.Name
                new Claim(ClaimTypes.Email, user.Email),          // Enables ClaimTypes.Email
                new Claim(ClaimTypes.Role, role),


            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));

            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
               issuer: _jwtSettings.Issuer,
               audience: _jwtSettings.Audience,
               claims: claims,
               expires: DateTime.UtcNow.AddMinutes(_jwtSettings.Duration),
               signingCredentials: signingCredentials);

            return jwtSecurityToken;
        }
        private async Task<AppUser?> GetVerifiedUserAsync(string username, string email)
        {
            var userByName = await _userManager.FindByNameAsync(username);
            var userByEmail = await _userManager.FindByEmailAsync(email);

            if (userByName == null || userByEmail == null) return null;
            return userByName.Id == userByEmail.Id ? userByName : null;
        }

        //private async Task UpdateUserPasswordAsync(AppUser user, string newPassword)
        //{
        //    var removeResult = await _userManager.RemovePasswordAsync(user);
        //    if (!removeResult.Succeeded)
        //    {
        //        HandleFailure("Password removal failed", removeResult.Errors, user.Id);
        //    }

        //    var addResult = await _userManager.AddPasswordAsync(user, newPassword);
        //    if (!addResult.Succeeded)
        //    {
        //        HandleFailure("Password update failed", addResult.Errors, user.Id);
        //    }

        //    _logger.LogInformation("Password updated successfully for User ID: {UserId}", user.Id);
        //}

        private async Task UpdateUserRoleAsync(AppUser user, string newRole)
        {
            if (string.IsNullOrWhiteSpace(newRole))
            {
                _logger.LogWarning("No role provided for update. Skipping role assignment.");
                return;
            }

            // Remove existing roles
            var currentRoles = await _userManager.GetRolesAsync(user);
            if (currentRoles.Any())
            {
                var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                if (!removeResult.Succeeded)
                {
                    HandleFailure("Failed to remove current roles", removeResult.Errors, user.Id);
                }
            }

            // Assign new role
            var addResult = await _userManager.AddToRoleAsync(user, newRole);
            if (!addResult.Succeeded)
            {
                HandleFailure($"Failed to assign role '{newRole}'", addResult.Errors, user.Id);
            }

            _logger.LogInformation("Role '{Role}' assigned to User ID: {UserId}", newRole, user.Id);
        }

        private void HandleFailure(string message, IEnumerable<IdentityError> errors, string userId)
        {
            var failures = errors
                .Select(e => new FluentValidation.Results.ValidationFailure("", e.Description))
                .ToList();

            var failureResult = new FluentValidation.Results.ValidationResult(failures);
            _logger.LogError("{Message} for ID {UserId}: {Errors}", message, userId, failures);
            throw new BadRequestException(message, failureResult);
        }

        private void LogValidation(string context, IEnumerable<FluentValidation.Results.ValidationFailure> errors)
        {
            _logger.LogWarning("Validation failed during {Context}: {Errors}", context, errors);
        }

        private CustomResultResponse Fail(string message)
        {
            return new CustomResultResponse { IsSuccess = false, Message = message };
        }

        private string GenerateDefaultPassword(string firstName, string lastName, int currentYear)
        {
            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
                throw new ArgumentException("First name and last name are required for password generation.");

            // Capitalize first initial
            var firstInitial = char.ToUpper(firstName.Trim()[0]);

            // Convert last name to Title Case
            var titleCaseLastName = string.Join("",
                lastName.Trim()
                        .Split(" ", StringSplitOptions.RemoveEmptyEntries)
                        .Select(word => char.ToUpper(word[0]) + word.Substring(1).ToLower()));

            var generatedUsername = $"{firstInitial}{titleCaseLastName}";
            var generatedPassword = $"{generatedUsername}@{currentYear}";

            return generatedPassword;
        }


    }
}
