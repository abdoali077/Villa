using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Villla.Domain.Entities;

namespace Villla.Application.Dtos
{
   
        public class AmenityDto
        {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int VillaId { get; set; }
        public string VillaName { get; set; } = string.Empty;
        public bool IsSelected { get; set; }

        [ValidateNever]
            public IEnumerable<SelectListItem> VillaList { get; set; }
                = new List<SelectListItem>();
        }
    
}
