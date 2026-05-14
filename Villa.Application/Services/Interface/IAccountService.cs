using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Villla.Application.Dtos;

namespace Villla.Application.Services.Interface
{
    public interface IAccountService
    {
        Task<AuthResultDto> RegisterAsync(RegisterDto dto, string scheme,
       Func<string, string, object, string, string> urlHelper);

        Task<AuthResultDto> LoginAsync(LoginDto dto);

        Task<bool> ConfirmEmailAsync(string userId, string token);

        Task LogoutAsync();

        Task<List<SelectListItem>> GetRolesAsync();

        Task EnsureRolesCreatedAsync();
    }
}
