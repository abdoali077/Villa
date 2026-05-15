using Microsoft.AspNetCore.Mvc;
using Villla.Application.Dtos;
using Villla.Application.Services.Interface;

namespace Villla.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHomeService _homeService;

        public HomeController(IHomeService homeService)
        {
            _homeService = homeService;
        }

        // ================= GET =================
        public async Task<IActionResult> Index(int? villaId, int? scroll)
        {
            var dto = await _homeService.GetHomeDataAsync(villaId, scroll);

            ViewBag.VillaId = villaId;
            ViewBag.Scroll = scroll;

            return View(dto);
        }

        // ================= POST =================
        [HttpPost]
        public async Task<IActionResult> Index(HomeDto dto, int? scroll)
        {
            var result = await _homeService.FilterHomeDataAsync(dto);

            ViewBag.Scroll = scroll;

            return View(result);
        }

        // ================= AJAX =================
        [HttpPost]
        public async Task<IActionResult> GetVillaByDate(int nights, DateOnly checkInDate)
        {
            var result = await _homeService.GetVillasByDateAsync(nights, checkInDate);

            return PartialView("_VillaList", result);
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}