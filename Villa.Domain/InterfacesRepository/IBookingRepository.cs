using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Villla.Domain.Entities;

namespace Villla.Application.Interfaces.CommonRepos
{
    public interface IBookingRepository : IRepository<Booking>
    {
         void UpdateBooking(Booking entity);
          void UpdateStatus (int bookingId, string bookingStatus,int? villaNumber=null);
        void UpdateStripePaymentId(int bookingId,string sessionId ,string stripePaymentIntentId);
    }
}
