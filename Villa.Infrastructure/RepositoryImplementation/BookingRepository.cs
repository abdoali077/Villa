using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Villla.Application.Interfaces.CommonRepos;
using Villla.Application.Utility;
using Villla.Domain.Entities;
using Villla.Infrastructure.Data;

namespace Villla.Infrastructure.RepositoryImplementation
{
    public class BookingRepository : GenericRepository<Booking>, IBookingRepository
    {
        private readonly ApplicationDbContext _db;
        public BookingRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
       
        public void UpdateBooking(Booking entity)
        {
            _db.Bookings.Update(entity);
        }

        public void UpdateStatus(int bookingId, string bookingStatus, int? villaNumber = null)
        {
           var BookingFromDb = _db.Bookings.FirstOrDefault(b => b.Id == bookingId);
            if (BookingFromDb != null)
            {
                BookingFromDb.Status = bookingStatus;
                if(bookingStatus == BookingStatus.CheckIn)
                {
                    BookingFromDb.ActualCheckInDate = DateTime.Now;
                    BookingFromDb.VillaNumber = villaNumber;
                }
                else if(bookingStatus == BookingStatus.Completed)
                {
                    BookingFromDb.ActualCheckOutDate = DateTime.Now;
                }

            }
        }

        public void UpdateStripePaymentId(int bookingId, string sessionId, string stripePaymentIntentId)
        {
            var BookingFromDb = _db.Bookings.FirstOrDefault(b => b.Id == bookingId);
            if (BookingFromDb != null)
            {
                if(!string.IsNullOrEmpty(sessionId))
                {
                    BookingFromDb.StripeSessionId = sessionId;
                }

                if (!string.IsNullOrEmpty(stripePaymentIntentId))
                {
                    BookingFromDb.StripePaymentIntentId = stripePaymentIntentId;
                    BookingFromDb.PaymentDate = DateTime.Now;
                    BookingFromDb.IsPaymentSuccessful = true;
                }
                    
            }
        }
    }
}
