using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Villla.Application.Services.Interface;

namespace Villla.Web.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        public IActionResult Index()
        {
            return View();
        }

        // ================= BOOKINGS =================
        [HttpGet]
        public async Task<IActionResult> GetTotalBookingsData()
        {
            var data = await _dashboardService.GetBookingsDataAsync();
            return Json(data);
        }

        // ================= USERS =================
        [HttpGet]
        public async Task<IActionResult> GetTotalRegisterUserData()
        {
            var data = await _dashboardService.GetUsersDataAsync();
            return Json(data);
        }

        // ================= REVENUE =================
        [HttpGet]
        public async Task<IActionResult> GetTotalRevenueData()
        {
            var data = await _dashboardService.GetRevenueDataAsync();
            return Json(data);
        }

        // ================= PIE CHART =================
        [HttpGet]
        public async Task<IActionResult> GetCustomerBookingPieChart()
        {
            var data = await _dashboardService.GetCustomerPieChartAsync();
            return Json(data);
        }
    }
}