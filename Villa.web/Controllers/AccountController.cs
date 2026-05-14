//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Identity.UI.Services;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Rendering;
//using Microsoft.AspNetCore.WebUtilities;
//using System.Text;
//using Villla.Application.Interfaces.CommonRepos;
//using Villla.Application.Interfaces.Services;
//using Villla.Application.Utility;
//using Villla.Domain.Entities;
//using Villla.Web.ViewModels;

//namespace Villla.Web.Controllers
//{

//    public class AccountController : Controller
//    {
//        private readonly IUnitOfWork _uow;
//        private readonly UserManager<ApplicationUser> _userManager; 
//        private readonly SignInManager<ApplicationUser> _signInManager;
//        private readonly RoleManager<IdentityRole> _roleManager;
//        private readonly IEmailService _emailService;
//        public AccountController(IUnitOfWork uow,
//            UserManager<ApplicationUser> userManager,
//            SignInManager<ApplicationUser> signInManager,
//            RoleManager<IdentityRole> roleManager , IEmailService emailService)
//        {
//            _uow = uow;
//            _userManager = userManager;
//            _signInManager = signInManager;
//            _roleManager = roleManager;
//            _emailService = emailService;
//        }

//        [Authorize]
//        public async Task<IActionResult> Logout()
//        {
//                await _signInManager.SignOutAsync();
//                return RedirectToAction("Index", "Home");
//        }
//        public IActionResult AccessDenied()
//        {
//            return View();
//        }

//        //-------------------------------------------
//        [AllowAnonymous]
//        public async Task<IActionResult> Register(string? returnUrl)
//        {

//            if (!await _roleManager.RoleExistsAsync(SD.Role_Admin))
//            {
//                await _roleManager.CreateAsync(new IdentityRole { Name = SD.Role_Admin });
//                await _roleManager.CreateAsync(new IdentityRole { Name = SD.Role_Customer });
//            }

//            var model = new RegisterVM
//            {
//                ReturnUrl = returnUrl,
//                RoleList = _roleManager.Roles
//                    .Select(u => new SelectListItem
//                    {
//                        Text = u.Name,
//                        Value = u.Name
//                    }).ToList()
//            };
//            return View(model);
//        }
//        [HttpPost]
//        [AllowAnonymous]
//        public async Task<IActionResult> Register(RegisterVM model)
//            {
//                // =========================
//                // 1. VALIDATION CHECK
//                // =========================
//                if (!ModelState.IsValid)
//                {
//                    model.RoleList = _roleManager.Roles
//                        .Select(u => new SelectListItem
//                        {
//                            Text = u.Name,
//                            Value = u.Name
//                        }).ToList();

//                    return View(model);
//                }

//                // =========================
//                // 2. CHECK EMAIL EXISTS
//                // =========================
//                var existingUser = await _userManager.FindByEmailAsync(model.Email);

//                if (existingUser != null)
//                {
//                    ModelState.AddModelError("Email", "Email is already in use.");

//                    model.RoleList = _roleManager.Roles
//                        .Select(u => new SelectListItem
//                        {
//                            Text = u.Name,
//                            Value = u.Name
//                        }).ToList();

//                    return View(model);
//                }

//                // =========================
//                // 3. CREATE USER
//                // =========================
//                var user = new ApplicationUser
//                {
//                    UserName = model.Email,
//                    Email = model.Email,
//                    Name = model.Name,
//                    PhoneNumber = model.PhoneNumber,
//                    CreatedAt = DateTime.UtcNow

//                };

//                // =========================
//                // 4. SAVE USER IN DB
//                // =========================
//                var result = await _userManager.CreateAsync(user, model.Password);

//                if (result.Succeeded)
//                {
//                    // =========================
//                    // 5. ASSIGN ROLE
//                    // =========================
//                    if (!string.IsNullOrEmpty(model.Role) &&
//                        await _roleManager.RoleExistsAsync(model.Role))
//                    {
//                        await _userManager.AddToRoleAsync(user, model.Role);
//                    }
//                    else
//                    {
//                        await _userManager.AddToRoleAsync(user, SD.Role_Customer);
//                    }

//                    // =========================
//                    // 6. GENERATE EMAIL TOKEN
//                    // =========================
//                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

//                    // Encode token (IMPORTANT)
//                    var encodedToken = WebEncoders.Base64UrlEncode(
//                        Encoding.UTF8.GetBytes(token)
//                    );

//                    // =========================
//                    // 7. CREATE CONFIRMATION LINK
//                    // =========================
//                    var confirmationLink = Url.Action(
//                        "ConfirmEmail",
//                        "Account",
//                        new { userId = user.Id, token = encodedToken },
//                        Request.Scheme
//                    );

//                string emailBody = $@"
//<div style='font-family: Arial, sans-serif; background-color: #f4f7f6; padding: 40px 0; width: 100%;'>
//    <table align='center' border='0' cellpadding='0' cellspacing='0' width='600' style='background-color: #ffffff; border-radius: 12px; overflow: hidden; shadow: 0 4px 12px rgba(0,0,0,0.1);'>
//        <tr>
//            <td align='center' style='padding: 30px 0; background-color: #0d6efd;'>
//                <h1 style='color: #ffffff; margin: 0; font-size: 28px; font-weight: bold;'>VillaWeb Luxury</h1>
//            </td>
//        </tr>
//        <tr>
//            <td style='padding: 40px 30px;'>
//                <h2 style='color: #333333; margin-top: 0;'>Welcome, {user.Name}!</h2>
//                <p style='color: #666666; font-size: 16px; line-height: 1.6;'>
//                    Thanks for joining our exclusive community. We're excited to help you find your next luxury getaway. To get started, please confirm your email address by clicking the button below:
//                </p>

//                <table border='0' cellpadding='0' cellspacing='0' style='margin: 30px auto;'>
//                    <tr>
//                        <td align='center' style='border-radius: 50px;' bgcolor='#0d6efd'>
//                            <a href='{confirmationLink}' target='_blank' style='padding: 15px 35px; font-size: 16px; font-family: Helvetica, Arial, sans-serif; color: #ffffff; text-decoration: none; border-radius: 50px; display: inline-block; font-weight: bold;'>
//                                Confirm Your Email
//                            </a>
//                        </td>
//                    </tr>
//                </table>

//                <p style='color: #666666; font-size: 14px; line-height: 1.6;'>
//                    If the button doesn't work, copy and paste this link into your browser: <br>
//                    <a href='{confirmationLink}' style='color: #0d6efd;'>{confirmationLink}</a>
//                </p>
//            </td>
//        </tr>
//        <tr>
//            <td style='padding: 20px 30px; background-color: #f8f9fa; text-align: center;'>
//                <p style='color: #999999; font-size: 12px; margin: 0;'>
//                    &copy; {DateTime.Now.Year} VillaWeb System. All rights reserved.<br>
//                    You received this email because you signed up for an account on our website.
//                </p>
//            </td>
//        </tr>
//    </table>
//</div>";

//                await _emailService.SendEmailAsync(user.Email, "Activate Your VillaWeb Account", emailBody);

//                // =========================
//                // 9. REDIRECT (NO LOGIN YET)
//                // =========================
//                return RedirectToAction("RegisterConfirmation");
//                }

//                // =========================
//                // 10. ERROR HANDLING
//                // =========================
//                foreach (var error in result.Errors)
//                {
//                    ModelState.AddModelError("", error.Description);
//                }

//                model.RoleList = _roleManager.Roles
//                    .Select(u => new SelectListItem
//                    {
//                        Text = u.Name,
//                        Value = u.Name
//                    }).ToList();

//                return View(model);
//            }
//        [AllowAnonymous]
//        public async Task<IActionResult> ConfirmEmail(string userId, string token)
//        {
//            if (userId == null || token == null)
//                return BadRequest();

//            var user = await _userManager.FindByIdAsync(userId);

//            if (user == null)
//                return NotFound();

//            var decodedToken = Encoding.UTF8.GetString(
//                WebEncoders.Base64UrlDecode(token)
//            );

//            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

//            if (result.Succeeded)
//                return View("ConfirmEmailSuccess");

//            return View("Error");
//        }
//        public IActionResult RegisterConfirmation()
//        {
//            return View();
//        }
//        [HttpGet]
//        [AllowAnonymous]
//        public IActionResult Login(string? returnUrl = null)
//        {
//            returnUrl ??= Url.Content("~/");

//            var model = new LoginVM
//            {
//                ReturnUrl = returnUrl
//            };

//            return View(model);
//        }
//        [HttpPost]
//        [AllowAnonymous]
//        public async Task<IActionResult> Login(LoginVM model)
//        {
//            if (!ModelState.IsValid)
//                return View(model);

//            // =========================
//            // 1. Find user
//            // =========================
//            var user = await _userManager.FindByEmailAsync(model.Email);

//            if (user == null)
//            {
//                ModelState.AddModelError("", "Invalid login attempt.");
//                return View(model);
//            }

//            // =========================
//            // 2. CHECK EMAIL CONFIRMATION
//            // =========================
//            if (!await _userManager.IsEmailConfirmedAsync(user))
//            {
//                ModelState.AddModelError("", "Please confirm your email first.");
//                return View(model);
//            }

//            // =========================
//            // 3. SIGN IN
//            // =========================
//            var result = await _signInManager.PasswordSignInAsync(
//                model.Email,
//                model.Password,
//                model.RememberMe,
//                lockoutOnFailure: false
//            );

//            if (result.Succeeded)
//            {
//                if(await _userManager.IsInRoleAsync(user, SD.Role_Admin))
//                {
//                    return RedirectToAction("Index", "Dashboard");
//                }
//                // =========================
//                // 4. REDIRECT LOGIC
//                // =========================
//                if (string.IsNullOrEmpty(model.ReturnUrl))
//                {
//                    return RedirectToAction("Index", "Home");
//                }

//                return LocalRedirect(model.ReturnUrl);
//            }

//            // =========================
//            // 5. FAILURE
//            // =========================
//            ModelState.AddModelError("", "Invalid login attempt.");

//            return View(model);
//        }
//    }
//}


using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Villla.Application.Dtos;
using Villla.Application.Interfaces.Services;
using Villla.Application.Services.Interface;

namespace Villla.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        // ================= LOGOUT =================
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _accountService.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        // ================= REGISTER (GET) =================
        [AllowAnonymous]
        public async Task<IActionResult> Register(string? returnUrl)
        {
            await _accountService.EnsureRolesCreatedAsync();

            var dto = new RegisterDto
            {
                ReturnUrl = returnUrl,
                RoleList = await _accountService.GetRolesAsync()
            };

            return View(dto);
        }

        // ================= REGISTER (POST) =================
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (!ModelState.IsValid)
            {
                dto.RoleList = await _accountService.GetRolesAsync();
                return View(dto);
            }

            var result = await _accountService.RegisterAsync(
                dto,
                Request.Scheme,
                (action, controller, values, scheme) =>
                    Url.Action(action, controller, values, scheme)!
            );

            if (!result.IsSuccess)
            {
                ModelState.AddModelError("", result.ErrorMessage);
                dto.RoleList = await _accountService.GetRolesAsync();
                return View(dto);
            }

            return Redirect(result.RedirectUrl!);
        }

        // ================= LOGIN (GET) =================
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            var dto = new LoginDto
            {
                ReturnUrl = returnUrl ?? Url.Content("~/")
            };

            return View(dto);
        }

        // ================= LOGIN (POST) =================
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var result = await _accountService.LoginAsync(dto);

            if (!result.IsSuccess)
            {
                ModelState.AddModelError("", result.ErrorMessage);
                return View(dto);
            }

            return Redirect(result.RedirectUrl!);
        }

        // ================= CONFIRM EMAIL =================
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
                return BadRequest();

            var success = await _accountService.ConfirmEmailAsync(userId, token);

            if (success)
                return View("ConfirmEmailSuccess");

            return View("Error");
        }

        public IActionResult RegisterConfirmation()
        {
            return View();
        }
    }
}
