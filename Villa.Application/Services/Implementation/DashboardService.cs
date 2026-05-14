using Villla.Application.Dtos;
using Villla.Application.Interfaces.CommonRepos;
using Villla.Application.Services.Interface;
using Villla.Application.Utility;

namespace Villla.Application.Services.Implementation
{
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _uow;

        public DashboardService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        // ================= BOOKINGS =================
        public async Task<RadialBarChartDto> GetBookingsDataAsync()
        {
            var today = DateTime.Now;

            var currentMonthStart = new DateTime(today.Year, today.Month, 1);
            var previousMonthStart = currentMonthStart.AddMonths(-1);
            var previousMonthEnd = currentMonthStart.AddDays(-1);

            var bookings = _uow.Bookings.GetAll(b =>
                b.Status != BookingStatus.Cancelled &&
                b.Status != BookingStatus.Pending);

            var total = bookings.Count();

            var current = bookings.Count(b =>
                b.BookingDate >= currentMonthStart &&
                b.BookingDate <= today);

            var previous = bookings.Count(b =>
                b.BookingDate >= previousMonthStart &&
                b.BookingDate <= previousMonthEnd);

            var diff = current - previous;

            decimal percent = 0;
            bool isIncrease = false;

            if (previous == 0 && current > 0)
            {
                percent = 100;
                isIncrease = true;
            }
            else if (previous > 0)
            {
                percent = ((decimal)(current - previous) / previous) * 100;
                isIncrease = percent > 0;
            }

            return new RadialBarChartDto
            {
                TotalCount = total,
                Difference = diff,
                HasRatioIncreases = isIncrease,
                IncreaseDecreaseAmount = Math.Round(percent, 2),
                Series = new decimal[]
                {
                    Math.Min(Math.Abs(percent), 100)
                }
            };
        }

        // ================= USERS =================
        public async Task<RadialBarChartDto> GetUsersDataAsync()
        {
            var today = DateTime.Now;

            var currentMonthStart = new DateTime(today.Year, today.Month, 1);
            var previousMonthStart = currentMonthStart.AddMonths(-1);
            var previousMonthEnd = currentMonthStart.AddDays(-1);

            var users = _uow.ApplicationUsers.GetAll(u => u.CreatedAt <= today);

            var total = users.Count();

            var current = users.Count(u =>
                u.CreatedAt >= currentMonthStart &&
                u.CreatedAt <= today);

            var previous = users.Count(u =>
                u.CreatedAt >= previousMonthStart &&
                u.CreatedAt <= previousMonthEnd);

            var diff = current - previous;

            decimal percent = 0;
            bool isIncrease = false;

            if (previous == 0 && current > 0)
            {
                percent = 100;
                isIncrease = true;
            }
            else if (previous > 0)
            {
                percent = ((decimal)(current - previous) / previous) * 100;
                isIncrease = percent > 0;
            }

            return new RadialBarChartDto
            {
                TotalCount = total,
                Difference = diff,
                HasRatioIncreases = isIncrease,
                IncreaseDecreaseAmount = Math.Round(percent, 2),
                Series = new decimal[] { Math.Min(Math.Abs(percent), 100) }
            };
        }

        // ================= REVENUE =================
        public async Task<RevenueRadialChartDto> GetRevenueDataAsync()
        {
            var today = DateTime.Now;

            var currentMonthStart = new DateTime(today.Year, today.Month, 1);
            var previousMonthStart = currentMonthStart.AddMonths(-1);
            var previousMonthEnd = currentMonthStart.AddDays(-1);

            var paidBookings = _uow.Bookings.GetAll(b =>
                b.Status == BookingStatus.Approved ||
                b.Status == BookingStatus.Completed ||
                b.Status == BookingStatus.CheckIn);

            var total = paidBookings.Sum(b => b.TotalCost);

            var current = paidBookings
                .Where(b => b.BookingDate >= currentMonthStart && b.BookingDate <= today)
                .Sum(b => b.TotalCost);

            var previous = paidBookings
                .Where(b => b.BookingDate >= previousMonthStart && b.BookingDate <= previousMonthEnd)
                .Sum(b => b.TotalCost);

            var diff = current - previous;

            decimal percent = 0;
            bool isIncrease = false;

            if (previous == 0 && current > 0)
            {
                percent = 100;
                isIncrease = true;
            }
            else if (previous > 0)
            {
                percent = ((current - previous) / previous) * 100;
                isIncrease = percent > 0;
            }

            return new RevenueRadialChartDto
            {
                TotalRevenue = total,
                Difference = diff,
                HasRatioIncreases = isIncrease,
                IncreaseDecreaseAmount = Math.Round(percent, 2),
                Series = new decimal[] { Math.Min(Math.Abs(percent), 100) }
            };
        }

        // ================= PIE CHART =================
        public async Task<PieChartDto> GetCustomerPieChartAsync()
        {
            var today = DateTime.Now;
            var last30Days = today.AddDays(-30);

            var bookings = _uow.Bookings.GetAll(b =>
                b.BookingDate >= last30Days &&
                b.BookingDate <= today &&
                b.Status != BookingStatus.Cancelled &&
                b.Status != BookingStatus.Pending);

            var previousCustomers = _uow.Bookings.GetAll(b =>
                b.BookingDate < last30Days)
                .Select(b => b.UserId)
                .Distinct()
                .ToList();

            var newCustomers = bookings
                .Where(b => !previousCustomers.Contains(b.UserId))
                .Select(b => b.UserId)
                .Distinct()
                .Count();

            var returningCustomers = bookings
                .Where(b => previousCustomers.Contains(b.UserId))
                .Select(b => b.UserId)
                .Distinct()
                .Count();

            return new PieChartDto
            {
                Labels = new[] { "New Customers", "Returning Customers" },
                Series = new decimal[] { newCustomers, returningCustomers }
            };
        }
    }
}