using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Villla.Application.Dtos
{
    public class RegisterDto
    {
        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string ConfirmPassword { get; set; } = string.Empty; // ✅ ADD

        public string? PhoneNumber { get; set; }

        public string? Role { get; set; }

        public string? ReturnUrl { get; set; } // ✅ ADD

        public IEnumerable<SelectListItem> RoleList { get; set; } = new List<SelectListItem>(); // ✅ ADD
    }
}
