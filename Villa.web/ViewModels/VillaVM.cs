using System.ComponentModel.DataAnnotations;

namespace Villla.Web.ViewModels
{


    public class VillaViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string? Description { get; set; }

        public decimal Price { get; set; }

        // بيظهر في العرض فقط
        public string? ImageUrl { get; set; }

        // للرفع
        public IFormFile? Image { get; set; }

        // لو عندك Amenities
        public List<int>? SelectedAmenities { get; set; }

        // لعرض checkboxes
        public List<AmenityVM>? Amenities { get; set; }
    }
}
