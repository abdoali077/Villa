using System.ComponentModel.DataAnnotations;

namespace Villla.Web.ViewModels
{
    public class FinalizeBookingVM
    {
        public int VillaId { get; set; }
        public string VillaName { get; set; }
        public string ImageUrl { get; set; }

        public DateTime CheckInDate { get; set; }
        public int Nights { get; set; }
        public DateTime CheckOutDate { get; set; }

        public decimal PricePerNight { get; set; }
        public decimal TotalCost { get; set; }

        public string Name { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        [Phone]
        public string? PhoneNumber { get; set; }
        public bool IsAvailable { get; set; }   
        public string UserId { get; set; }
        public string Status { get; set; }
    }
}
