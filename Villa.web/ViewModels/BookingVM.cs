using System.ComponentModel.DataAnnotations;

namespace Villla.Web.ViewModels
{
    public class BookingVM
    {
        public int BookingId { get; set; }

        public string Name { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        [Phone]
        public string PhoneNumber { get; set; }

        public int Nights { get; set; }
        public decimal TotalCost { get; set; }
        public string Status { get; set; }
        public DateOnly CheckInDate { get; set; }


    }
}
