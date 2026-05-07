using Microsoft.AspNetCore.Mvc.Rendering;
using Villla.Domain.Entities;

namespace Villla.Web.ViewModels.VillaNumberVM
{
    public class VillaNumberVM
    {
        public VillaNumber VillaNumber { get; set; }
        public IEnumerable<SelectListItem>? VillaList { get; set; }
        public string? SpecialDetails { get; set; }
    }
}
