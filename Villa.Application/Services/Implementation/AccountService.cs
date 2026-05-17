using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System.Text;
using Villla.Application.Dtos;
using Villla.Application.Services.Interface;
using Villla.Application.Utility;
using Villla.Domain.Entities;

namespace Villla.Application.Services.Implementation
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailService _emailService;
        private readonly ILogger<AccountService> _logger;

        public AccountService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            IEmailService emailService,
            ILogger<AccountService> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _emailService = emailService;
            _logger = logger;
        }

        // ================= ROLES =================
        public async Task EnsureRolesCreatedAsync()
        {
            try
            {
                _logger.LogInformation("AccountService - Ensuring roles exist");

                if (!await _roleManager.RoleExistsAsync(SD.Role_Admin))
                {
                    await _roleManager.CreateAsync(new IdentityRole { Name = SD.Role_Admin });
                    await _roleManager.CreateAsync(new IdentityRole { Name = SD.Role_Customer });
                }

                _logger.LogInformation("AccountService - Roles checked/created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating roles");
                throw;
            }
        }

        public async Task<List<SelectListItem>> GetRolesAsync()
        {
            try
            {
                _logger.LogInformation("AccountService - Fetching roles");

                return _roleManager.Roles.Select(r => new SelectListItem
                {
                    Text = r.Name!,
                    Value = r.Name!
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching roles");
                throw;
            }
        }

        // ================= REGISTER =================
        public async Task<AuthResultDto> RegisterAsync(
            RegisterDto dto,
            string scheme,
            Func<string, string, object, string, string> urlHelper)
        {
            try
            {
                _logger.LogInformation("AccountService - Register started for {Email}", dto.Email);

                if (dto.Password != dto.ConfirmPassword)
                {
                    return new AuthResultDto
                    {
                        IsSuccess = false,
                        ErrorMessage = "Passwords do not match."
                    };
                }

                var existingUser = await _userManager.FindByEmailAsync(dto.Email);
                if (existingUser != null)
                {
                    return new AuthResultDto
                    {
                        IsSuccess = false,
                        ErrorMessage = "Email is already in use."
                    };
                }

                var user = new ApplicationUser
                {
                    UserName = dto.Email,
                    Email = dto.Email,
                    Name = dto.Name,
                    PhoneNumber = dto.PhoneNumber,
                    CreatedAt = DateTime.UtcNow
                };

                var result = await _userManager.CreateAsync(user, dto.Password);

                if (!result.Succeeded)
                {
                    return new AuthResultDto
                    {
                        IsSuccess = false,
                        ErrorMessage = string.Join(", ", result.Errors.Select(e => e.Description))
                    };
                }

                // ROLE
                if (!string.IsNullOrEmpty(dto.Role) &&
                    await _roleManager.RoleExistsAsync(dto.Role))
                {
                    await _userManager.AddToRoleAsync(user, dto.Role);
                }
                else
                {
                    await _userManager.AddToRoleAsync(user, SD.Role_Customer);
                }

                // EMAIL CONFIRMATION
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                var encodedToken = WebEncoders.Base64UrlEncode(
                    Encoding.UTF8.GetBytes(token)
                );

                var confirmationLink = urlHelper(
                    "ConfirmEmail",
                    "Account",
                    new { userId = user.Id, token = encodedToken },
                    scheme
                );

                string emailBody = $@"
<div style='font-family: Arial, sans-serif; background-color: #f4f7f6; padding: 40px 0; width: 100%;'>
    <table align='center' border='0' cellpadding='0' cellspacing='0' width='600' style='background-color: #ffffff; border-radius: 12px; overflow: hidden; shadow: 0 4px 12px rgba(0,0,0,0.1);'>
        <tr>
            <td align='center' style='padding: 30px 0; background-color: #0d6efd;'>
                <h1 style='color: #ffffff; margin: 0; font-size: 28px; font-weight: bold;'>VillaWeb Luxury</h1>
            </td>
        </tr>
        <tr>
            <td style='padding: 40px 30px;'>
                <h2 style='color: #333333; margin-top: 0;'>Welcome, {user.Name}!</h2>
                <p style='color: #666666; font-size: 16px; line-height: 1.6;'>
                    Thanks for joining our exclusive community. We're excited to help you find your next luxury getaway. To get started, please confirm your email address by clicking the button below:
                </p>

                <table border='0' cellpadding='0' cellspacing='0' style='margin: 30px auto;'>
                    <tr>
                        <td align='center' style='border-radius: 50px;' bgcolor='#0d6efd'>
                            <a href='{confirmationLink}' target='_blank' style='padding: 15px 35px; font-size: 16px; font-family: Helvetica, Arial, sans-serif; color: #ffffff; text-decoration: none; border-radius: 50px; display: inline-block; font-weight: bold;'>
                                Confirm Your Email
                            </a>
                        </td>
                    </tr>
                </table>

                <p style='color: #666666; font-size: 14px; line-height: 1.6;'>
                    If the button doesn't work, copy and paste this link into your browser: <br>
                    <a href='{confirmationLink}' style='color: #0d6efd;'>{confirmationLink}</a>
                </p>
            </td>
        </tr>
        <tr>
            <td style='padding: 20px 30px; background-color: #f8f9fa; text-align: center;'>
                <p style='color: #999999; font-size: 12px; margin: 0;'>
                    &copy; {DateTime.Now.Year} VillaWeb System. All rights reserved.<br>
                    You received this email because you signed up for an account on our website.
                </p>
            </td>
        </tr>
    </table>
</div>";

                await _emailService.SendEmailAsync(
                    user.Email!,
                    "Confirm your email",
                    emailBody
                );

                _logger.LogInformation("AccountService - Register completed for {Email}", dto.Email);

                return new AuthResultDto
                {
                    IsSuccess = true,
                    RedirectUrl = "/Account/RegisterConfirmation"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration for {Email}", dto.Email);

                return new AuthResultDto
                {
                    IsSuccess = false,
                    ErrorMessage = "Something went wrong during registration."
                };
            }
        }

        // ================= LOGIN =================
        public async Task<AuthResultDto> LoginAsync(LoginDto dto)
        {
            try
            {
                _logger.LogInformation("AccountService - Login attempt for {Email}", dto.Email);

                var user = await _userManager.FindByEmailAsync(dto.Email);

                if (user == null)
                {
                    return new AuthResultDto
                    {
                        IsSuccess = false,
                        ErrorMessage = "Invalid login attempt."
                    };
                }

                if (!await _userManager.IsEmailConfirmedAsync(user))
                {
                    return new AuthResultDto
                    {
                        IsSuccess = false,
                        ErrorMessage = "Please confirm your email first."
                    };
                }

                var result = await _signInManager.PasswordSignInAsync(
                    dto.Email,
                    dto.Password,
                    dto.RememberMe,
                    false
                );

                if (!result.Succeeded)
                {
                    return new AuthResultDto
                    {
                        IsSuccess = false,
                        ErrorMessage = "Invalid login attempt."
                    };
                }

                if (await _userManager.IsInRoleAsync(user, SD.Role_Admin))
                {
                    return new AuthResultDto
                    {
                        IsSuccess = true,
                        RedirectUrl = "/Dashboard"
                    };
                }

                return new AuthResultDto
                {
                    IsSuccess = true,
                    RedirectUrl = dto.ReturnUrl ?? "/"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for {Email}", dto.Email);

                return new AuthResultDto
                {
                    IsSuccess = false,
                    ErrorMessage = "Login failed due to server error."
                };
            }
        }

        // ================= CONFIRM EMAIL =================
        public async Task<bool> ConfirmEmailAsync(string userId, string token)
        {
            try
            {
                _logger.LogInformation("AccountService - Confirm email for user {UserId}", userId);

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return false;

                var decodedToken = Encoding.UTF8.GetString(
                    WebEncoders.Base64UrlDecode(token)
                );

                var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

                return result.Succeeded;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error confirming email for user {UserId}", userId);
                return false;
            }
        }

        // ================= LOGOUT =================
        public async Task LogoutAsync()
        {
            try
            {
                _logger.LogInformation("AccountService - Logout");

                await _signInManager.SignOutAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
                throw;
            }
        }
    }
}