using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Villla.Application.Interfaces.CommonRepos;
using Villla.Application.Utility;
using Villla.Web.ViewModels;

namespace Villla.Web.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IUnitOfWork _uow;

        public DashboardController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetTotalBookingsData()
        {
            var today = DateTime.Now;

            // Current Month Start
            var currentMonthStartDate =
                new DateTime(today.Year, today.Month, 1);

            // Previous Month Start
            var previousMonthDate =
                currentMonthStartDate.AddMonths(-1);

            var previousMonthStartDate =
                new DateTime(previousMonthDate.Year,
                             previousMonthDate.Month, 1);

            // Previous Month End
            var previousMonthEndDate =
                currentMonthStartDate.AddDays(-1);

            var bookings = _uow.Bookings.GetAll(u =>
                u.Status != BookingStatus.Cancelled &&
                u.Status != BookingStatus.Pending);

            // Total Valid Bookings
            var totalBookings = bookings.Count();

            // Current Month Bookings
            var countByCurrentMonth = bookings.Count(u =>
                u.BookingDate >= currentMonthStartDate &&
                u.BookingDate <= today);

            // Previous Month Bookings
            var countByPreviousMonth = bookings.Count(u =>
                u.BookingDate >= previousMonthStartDate &&
                u.BookingDate <= previousMonthEndDate);

            // Difference Count
            int bookingsDifference =
                countByCurrentMonth - countByPreviousMonth;

            decimal increaseDecreaseAmount = 0;
            bool hasRatioIncrease = false;

            // If previous month = 0 and current > 0
            if (countByPreviousMonth == 0 && countByCurrentMonth > 0)
            {
                increaseDecreaseAmount = 100;
                hasRatioIncrease = true;
            }
            else if (countByPreviousMonth > 0)
            {
                increaseDecreaseAmount =
                    ((decimal)(countByCurrentMonth - countByPreviousMonth)
                    / countByPreviousMonth) * 100;

                if (increaseDecreaseAmount > 0)
                {
                    hasRatioIncrease = true;
                }
            }

            // Max chart value = 100
            decimal[] series = new decimal[]
            {
                Math.Min(Math.Abs(increaseDecreaseAmount), 100)
            };

            var result = new RadialBarChartVM
            {
                TotalCount = totalBookings,
                IncreaseDecreaseAmount = Math.Round(increaseDecreaseAmount, 2),
                HasRatioIncreases = hasRatioIncrease,
                Series = series,
                BookingsDifference = bookingsDifference
            };

            return Json(result);
        }
    }
}