using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Villla.Application.Dtos
{
    public class VillaNumberDto
    {
        public int VillaNumber { get; set; }

        public int VillaId { get; set; }

        public string VillaName { get; set; } = string.Empty; 

        public string? SpecialDetails { get; set; }

        // Dropdown
        public IEnumerable<SelectListItem> VillaList { get; set; }
            = new List<SelectListItem>();
    }
}
