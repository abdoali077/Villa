using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Villla.Domain.Entities;

namespace Villla.Web.ViewModels
{
    public class BookingDetailsVM
    {
      
        public int bookingId { get; set; }

        public string Name { get; set; }

    
        public string Email { get; set; }

        public string? PhoneNumber { get; set; }
        public decimal TotalCost { get; set; }

   
        public int Nights { get; set; }

        public string Status { get; set; }

        public DateTime BookingDate { get; set; }

        
        public DateOnly CheckInDate { get; set; }

        public Villla.Domain.Entities.Villa Villa { get; set; }
        public DateOnly CheckOutDate { get; set; }

        public bool IsPaymentSuccessful { get; set; }

        public DateTime? PaymentDate { get; set; }

        public string? StripeSessionId { get; set; }

        public string? StripePaymentIntentId { get; set; }

        public DateTime? ActualCheckInDate { get; set; }

        public DateTime? ActualCheckOutDate { get; set; }
        public List<VillaNumber>? VillaNumbers { get; set; }= new List<VillaNumber>();
        public int? VillaNumber { get; set; }
    }
}
