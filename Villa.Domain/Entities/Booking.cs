using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Villla.Domain.Entities
{
    public class Booking
    {
        [Key]
        public int Id { get; set; }

        // ===============================
        // User Relation
        // ===============================
        [Required]
        public string UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; }

        // ===============================
        // Villa Relation
        // ===============================
        [Required]
        public int VillaId { get; set; }

        [ForeignKey(nameof(VillaId))]
        public Villa Villa { get; set; }

        // ===============================
        // Customer Info
        // ===============================
        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string? PhoneNumber { get; set; }

        // ===============================
        // Booking Details
        // ===============================
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalCost { get; set; }

        [Required]
        public int Nights { get; set; }

        public string Status { get; set; } 

        // ===============================
        // Dates
        // ===============================
        public DateTime BookingDate { get; set; }

        [Required]
        public DateOnly CheckInDate { get; set; }

        [Required]
        public DateOnly CheckOutDate { get; set; }

        // ===============================
        // Payment
        // ===============================
        public bool IsPaymentSuccessful { get; set; } 

        public DateTime? PaymentDate { get; set; }

        public string? StripeSessionId { get; set; }

        public string? StripePaymentIntentId { get; set; }

        // ===============================
        // Real Stay Dates
        // ===============================
        public DateTime? ActualCheckInDate { get; set; }

        public DateTime? ActualCheckOutDate { get; set; }

        // ===============================
        // Assigned Villa Number
        // ===============================
        public int? VillaNumber { get; set; }
        [NotMapped]
        public List<VillaNumber>? VillaNumbers { get; set; }
    }
}
