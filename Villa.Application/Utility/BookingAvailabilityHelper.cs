using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Villla.Domain.Entities;

namespace Villla.Application.Utility
{
    public static class BookingAvailabilityHelper
    {
        public static int GetAvailableRoomsCount(
     Villa villa,
     List<VillaNumber> villaNumberList,
     DateOnly checkInDate,
     int nights,
     List<Booking> bookings)
        {
            if (villa == null)
                return 0;

            if (villaNumberList == null || !villaNumberList.Any())
                return 0;

            if (nights <= 0)
                return 0;

            int totalRooms = villaNumberList
                .Count(u => u.VillaId == villa.Id);

            if (totalRooms == 0)
                return 0;

            int minAvailableRooms = totalRooms;

            // فلترة مرة واحدة فقط (Optimization مهم جدًا)
            var villaBookings = bookings
                .Where(b =>
                    b.VillaId == villa.Id &&
                    b.Status != BookingStatus.Cancelled)
                .ToList();

            for (int i = 0; i < nights; i++)
            {
                DateOnly currentDate = checkInDate.AddDays(i);

                int bookedRoomsCount = villaBookings
                    .Where(b =>
                        b.CheckInDate <= currentDate &&
                        b.CheckOutDate > currentDate)
                    .Count();

                int availableRooms = totalRooms - bookedRoomsCount;

                if (availableRooms <= 0)
                    return 0;

                if (availableRooms < minAvailableRooms)
                    minAvailableRooms = availableRooms;
            }

            return minAvailableRooms;
        }

    }
}
