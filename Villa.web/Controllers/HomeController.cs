using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Villa.web.Models;
using Villla.Application.Interfaces.CommonRepos;
using Villla.Application.Utility;
using Villla.Domain.Entities;
using Villla.Web.ViewModels;

namespace Villa.web.Controllers
{
    public class HomeController : Controller
    {
      private readonly IUnitOfWork _uow;
        public HomeController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public IActionResult Index(int? villaId,int? scroll)
        {
            HomeVM vm = new HomeVM()
            {
                VillaList = _uow.Villas.GetAll(include:q=>q.Include(v=>v.Amenities)),
                Nights = 1,
                CheckInDate=DateOnly.FromDateTime(DateTime.Now)
            };
            ViewBag.VillaId = villaId;
            ViewBag.Scroll = scroll;


            return View(vm);
        }
        [HttpPost]
        public IActionResult Index(HomeVM? homeVM, int? scroll)
        {
            if (homeVM == null)
                homeVM = new HomeVM();

            if (homeVM.Nights <= 0)
                homeVM.Nights = 1;

            if (homeVM.CheckInDate < DateOnly.FromDateTime(DateTime.Today))
                homeVM.CheckInDate = DateOnly.FromDateTime(DateTime.Today);

            homeVM.VillaList = _uow.Villas
                .GetAll(include: q => q.Include(v => v.Amenities))
                .ToList();

            ViewBag.Scroll = scroll;

            return View(homeVM);
        }
        [HttpPost]
        public IActionResult GetVillaByDate(int nights,DateOnly checkInDate)
        {
            if (nights <= 0)
                nights = 1;

            if (checkInDate < DateOnly.FromDateTime(DateTime.Today))
                checkInDate = DateOnly.FromDateTime(DateTime.Today);

            var villas = _uow.Villas.GetAll(include:
                q => q.Include(v => v.Amenities));
            var villaNumbers = _uow.VillaNumbers.GetAll().ToList();
            var bookings = _uow.Bookings.GetAll(b => b.Status == BookingStatus.Approved || b.Status== BookingStatus.CheckIn).ToList();

            foreach (var villa in villas)
            {
                int availableRooms = BookingAvailabilityHelper.GetAvailableRoomsCount(villa, villaNumbers, checkInDate, nights, bookings);
                villa.IsAvailable = availableRooms > 0? true : false;
            }

            HomeVM vm = new()
            {
                VillaList = villas,
                Nights = nights,
                CheckInDate = checkInDate
            };

            //return View(vm);
            return PartialView("_VillaList", vm);
        }
        //var villaIds = _uow.Bookings.GetAll(q => q.CheckInDate >= checkInDate && q.CheckOutDate <= checkInDate.AddDays(nights)).Select(b => b.VillaId).ToList();
        //villas = villas.Where(v => !villaIds.Contains(v.Id)).ToList();
        //var villaNumbers = _uow.VillaNumbers.GetAll().ToList();
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
