using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using Villla.Application.Interfaces.CommonRepos;
using Villla.Application.Utility;
using Villla.Domain.Entities;
using Villla.Infrastructure.RepositoryImplementation;
using Villla.Web.ViewModels;

namespace Villla.Web.Controllers
{

    public class BookingController : Controller
    {
        private readonly IUnitOfWork _uow;
        public BookingController(IUnitOfWork uow)
        {
            _uow = uow;
        }
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }
        [Authorize]
        public async Task<IActionResult> FinalizeBooking(int villaId, DateTime checkInDate, int nights)
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account");

            var user = await _uow.ApplicationUsers.GetAsync(u => u.Id == userId);

            if (user == null)
                return Unauthorized();

            var villa = await _uow.Villas.GetAsync(
                v => v.Id == villaId,
                include: v => v.Include(q => q.Amenities));

            if (villa == null)
                return NotFound();
            // 🔥 Availability Check (IMPORTANT)
            var villaNumbers = (await _uow.VillaNumbers.GetAllAsync()).ToList();

            var bookings = (await _uow.Bookings.GetAllAsync(
                b => b.Status == BookingStatus.Approved ||
                     b.Status == BookingStatus.CheckIn
            )).ToList();

            int availableRooms = BookingAvailabilityHelper.GetAvailableRoomsCount(
                villa,
                villaNumbers,
                DateOnly.FromDateTime(checkInDate),
                nights,
                bookings
            );

            if (availableRooms <= 0)
            {
                TempData["error"] = "This villa is not available for selected dates.";
                return RedirectToAction("Index", "Home");
            }

            var vm = new FinalizeBookingVM
            {
                VillaId = villaId,
                VillaName = villa.Name,
                ImageUrl = villa.ImageUrl,

                CheckInDate = checkInDate,
                Nights = nights,
                CheckOutDate = checkInDate.AddDays(nights),

                PricePerNight = villa.Price,
                TotalCost = villa.Price * nights,
                IsAvailable = true,

                UserId = userId,
                Name = user.Name,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber
            };

            return View(vm);
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> FinalizeBooking(FinalizeBookingVM bookingVM)
        {
            var villa = await _uow.Villas.GetAsync(v => v.Id == bookingVM.VillaId);

            if (villa == null)
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // 🔥 Availability Check AGAIN (VERY IMPORTANT)
            var villaNumbers = (await _uow.VillaNumbers.GetAllAsync()).ToList();

            var bookings = (await _uow.Bookings.GetAllAsync(
                b => b.Status == BookingStatus.Approved ||
                     b.Status == BookingStatus.CheckIn
            )).ToList();

            int availableRooms = BookingAvailabilityHelper.GetAvailableRoomsCount(
                villa,
                villaNumbers,
                DateOnly.FromDateTime(bookingVM.CheckInDate),
                bookingVM.Nights,
                bookings
            );

            if (availableRooms <= 0)
            {
                TempData["error"] = "Villa became unavailable.";
                return RedirectToAction("Index", "Home");
            }

            var booking = new Booking
            {
                VillaId = bookingVM.VillaId,
                UserId = userId,
                Name = bookingVM.Name,
                Email = bookingVM.Email,
                PhoneNumber = bookingVM.PhoneNumber,

                BookingDate = DateTime.Now,

                Nights = bookingVM.Nights,
                TotalCost = villa.Price * bookingVM.Nights,

                Status = BookingStatus.Pending,

                CheckInDate = DateOnly.FromDateTime(bookingVM.CheckInDate),
                CheckOutDate = DateOnly.FromDateTime(
                    bookingVM.CheckInDate.AddDays(bookingVM.Nights)
                )
            };

            await _uow.Bookings.CreateAsync(booking);
            await _uow.SaveAsync();

            var domain = Request.Scheme + "://" + Request.Host.Value + "/";

            var options = new Stripe.Checkout.SessionCreateOptions
            {
                SuccessUrl = domain +
                             $"Booking/BookingConfirmation?bookingId={booking.Id}",

                CancelUrl = domain +
                            $"Booking/FinalizeBooking?villaId={booking.VillaId}" +
                            $"&checkInDate={bookingVM.CheckInDate:yyyy-MM-dd}" +
                            $"&nights={booking.Nights}",

                Mode = "payment",

                LineItems = new List<Stripe.Checkout.SessionLineItemOptions>()
            };

            options.LineItems.Add(new Stripe.Checkout.SessionLineItemOptions
            {
                Quantity = 1,

                PriceData = new Stripe.Checkout.SessionLineItemPriceDataOptions
                {
                    UnitAmount = (long)(booking.TotalCost * 100),
                    Currency = "usd",

                    ProductData = new Stripe.Checkout.SessionLineItemPriceDataProductDataOptions
                    {
                        Name = villa.Name
                    }
                }
            });

            var service = new Stripe.Checkout.SessionService();
            Stripe.Checkout.Session session = service.Create(options);
            _uow.Bookings.UpdateStripePaymentId(booking.Id, session.Id, session.PaymentIntentId);
            await _uow.SaveAsync();

            Response.Headers.Add("Location", session.Url);

            return new StatusCodeResult(303);
        }

        [Authorize]
        public async Task<IActionResult> BookingConfirmation(int bookingId)
        {
            var booking = await _uow.Bookings.GetAsync(b => b.Id == bookingId);

            if (booking == null)
                return NotFound();

            if (!string.IsNullOrEmpty(booking.StripeSessionId))
            {
                var service = new Stripe.Checkout.SessionService();
                Stripe.Checkout.Session session =
                    service.Get(booking.StripeSessionId);

                // لو الدفع تم بنجاح
                if (session.PaymentStatus.ToLower() == "paid")
                {
                    _uow.Bookings.UpdateStripePaymentId(
                        booking.Id,
                        session.Id,
                        session.PaymentIntentId
                    );

                    _uow.Bookings.UpdateStatus(
                        booking.Id,
                        BookingStatus.Approved, 0
                    );

                    await _uow.SaveAsync();

                    // إعادة تحميل البيانات بعد التحديث
                    booking = await _uow.Bookings.GetAsync(b => b.Id == bookingId);
                }
            }

            return View(booking);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll(string status)
        {
            IEnumerable<BookingVM> bookings;
            if (User.IsInRole(SD.Role_Admin))
            {
                bookings = (await _uow.Bookings.GetAllAsync(include: b => b.Include(q => q.Villa).Include(q => q.User)))
                    .Select(b => new BookingVM
                    {
                        BookingId = b.Id,
                        Name = b.Villa.Name,
                        Email = b.Email,
                        PhoneNumber = b.PhoneNumber,
                        Nights = b.Nights,
                        TotalCost = b.TotalCost,
                        Status = b.Status.ToString(),
                        CheckInDate = b.CheckInDate
                    });

            }
            else
            {
                var claimIdentity = (ClaimsIdentity)User.Identity;
                var userId = claimIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                bookings = (await _uow.Bookings.GetAllAsync(b => b.UserId == userId, include: b => b.Include(q => q.Villa)))
                    .Select(b => new BookingVM
                    {
                        BookingId = b.Id,
                        Name = b.Villa.Name,
                        Email = b.Email,
                        PhoneNumber = b.PhoneNumber,
                        Nights = b.Nights,
                        TotalCost = b.TotalCost,
                        Status = b.Status.ToString(),
                        CheckInDate = b.CheckInDate
                    });
            }
            if (!string.IsNullOrEmpty(status))
            {
                bookings = bookings.Where(b => b.Status.ToLower().Equals(status.ToLower()));
            }
            return Json(new { data = bookings });
        }

        [Authorize]
        public async Task<IActionResult> BookingDetails(int bookingId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var bookingQuery = await _uow.Bookings.GetAllAsync(
                b => b.Id == bookingId,
                include: b => b.Include(q => q.Villa)
            );

            if (!User.IsInRole(SD.Role_Admin))
            {
                bookingQuery = bookingQuery.Where(b => b.UserId == userId);
            }

            var booking = bookingQuery.FirstOrDefault();

            if (booking == null)
                return NotFound();

            // ================= ADMIN =================
            if (User.IsInRole(SD.Role_Admin))
            {
                var adminVM = new BookingDetailsVM
                {
                    Villa = booking.Villa,
                    bookingId = booking.Id,
                    Name = booking.Villa.Name,
                    Email = booking.Email,
                    PhoneNumber = booking.PhoneNumber,
                    Nights = booking.Nights,
                    VillaNumber = booking.VillaNumber,
                    TotalCost = booking.TotalCost,
                    Status = booking.Status.ToString(),
                    CheckInDate = booking.CheckInDate,
                    CheckOutDate = booking.CheckOutDate,
                    BookingDate = booking.BookingDate,
                    IsPaymentSuccessful = booking.Status == BookingStatus.Approved,
                    PaymentDate = booking.PaymentDate,
                    StripeSessionId = booking.StripeSessionId,
                    StripePaymentIntentId = booking.StripePaymentIntentId,
                    ActualCheckInDate = booking.ActualCheckInDate,
                    ActualCheckOutDate = booking.ActualCheckOutDate,

                    // IMPORTANT: always initialize
                    VillaNumbers = new List<VillaNumber>()
                };

                // Only when allowed to assign villa number
                if (booking.Status == BookingStatus.Approved)
                {
                    var availableVillaNumber = await AssignAvailableVillaNumbersByVilla(booking.VillaId);

                    adminVM.VillaNumbers = (await _uow.VillaNumbers.GetAllAsync(vn =>
                            vn.VillaId == booking.VillaId &&
                            availableVillaNumber.Contains(vn.Villa_Number) &&
                            vn.Villa_Number > 0
                        ))  
                        .ToList();
                }

                return View("BookingDetails", adminVM);
            }

            // ================= USER =================
            var userVM = new BookingDetailsVM
            {
                Villa = booking.Villa,
                bookingId = booking.Id,
                Name = booking.Villa.Name,
                Email = booking.Email,
                PhoneNumber = booking.PhoneNumber,
                Nights = booking.Nights,
                TotalCost = booking.TotalCost,
                Status = booking.Status.ToString(),
                CheckInDate = booking.CheckInDate,
                CheckOutDate = booking.CheckOutDate,
                BookingDate = booking.BookingDate,
                PaymentDate = booking.PaymentDate,
                ActualCheckInDate = booking.ActualCheckInDate,
                ActualCheckOutDate = booking.ActualCheckOutDate,
                IsPaymentSuccessful = booking.Status == BookingStatus.Approved,

                StripeSessionId = null,
                StripePaymentIntentId = null,

                VillaNumbers = new List<VillaNumber>() // safety
            };

            return View("BookingDetails", userVM);
        }

        private async Task<List<int>> AssignAvailableVillaNumbersByVilla(int villaId)
        {
            var villaNumbers = (await _uow.VillaNumbers.GetAllAsync(vn => vn.VillaId == villaId))
                .Where(vn => vn.Villa_Number > 0) // 🔥 prevent 0
                .ToList();

            var bookedNumbers = (await _uow.Bookings.GetAllAsync(b =>
                    b.VillaId == villaId &&
                    (b.Status == BookingStatus.Approved || b.Status == BookingStatus.CheckIn)))
                .Select(b => b.VillaNumber)
                .Where(x => x > 0) // 🔥 important
                .ToList();

            return villaNumbers
                .Where(vn => !bookedNumbers.Contains(vn.Villa_Number))
                .Select(vn => vn.Villa_Number)
                .ToList();
        }

        [Authorize(Roles = SD.Role_Admin)]
        [HttpPost]
        public async Task<IActionResult> CheckIn(BookingDetailsVM vm)
        {
            var booking = await _uow.Bookings.GetAsync(b => b.Id == vm.bookingId);

            if (booking == null)
                return NotFound();

            if (booking.Status != BookingStatus.Approved)
                return BadRequest("Booking is not approved yet");

            if (vm.VillaNumber == null || vm.VillaNumber == 0)
                return BadRequest("Please select villa number");

            if (!vm.VillaNumber.HasValue)
                return BadRequest("Villa number is required");

            int villaNumber = vm.VillaNumber.Value;

            var isTaken = (await _uow.Bookings.GetAllAsync(b =>
                    b.VillaId == booking.VillaId &&
                    b.VillaNumber == villaNumber &&
                    (b.Status == BookingStatus.CheckIn || b.Status == BookingStatus.Approved))) 
                .Any();

            if (isTaken)
                return BadRequest("Villa number already occupied");

            var available = await AssignAvailableVillaNumbersByVilla(booking.VillaId);

            if (!available.Contains(villaNumber))
                return BadRequest("Villa number no longer available");

            _uow.Bookings.UpdateStatus(booking.Id, BookingStatus.CheckIn, villaNumber);

            await _uow.SaveAsync();
            TempData["success"] = "Checked in successfully";

            return RedirectToAction("BookingDetails", new { bookingId = booking.Id });
        }

        //[Authorize(Roles = SD.Role_Admin)]
        //[HttpPost]
        //public async Task<IActionResult> CheckOut(BookingDetailsVM vm)
        //{
        //    var booking = await _uow.Bookings.GetAsync(b => b.Id == vm.bookingId);

        //    if (booking == null)
        //        return NotFound();

        //    if (booking.Status != BookingStatus.CheckIn)
        //        return BadRequest("Booking is not in CheckIn state");

        //    if (!vm.VillaNumber.HasValue)
        //        return BadRequest("Villa number is required");

        //    int villaNumber = vm.VillaNumber.Value;

        //    _uow.Bookings.UpdateStatus(booking.Id, BookingStatus.Completed, villaNumber);
        //    await _uow.SaveAsync();
        //    TempData["success"] = "Checked out successfully";

        //    return RedirectToAction("BookingDetails", new { bookingId = booking.Id });
        //}

        [Authorize(Roles = SD.Role_Admin)]
        [HttpPost]
        public async Task<IActionResult> CheckOut(BookingDetailsVM vm)
        {
            var booking = await _uow.Bookings.GetAsync(b => b.Id == vm.bookingId);

            if (booking == null)
                return NotFound();

            if (booking.Status != BookingStatus.CheckIn)
                return BadRequest("Booking is not in CheckIn state");

            var villaNumber = booking.VillaNumber;

            if (villaNumber == null || villaNumber == 0)
                return BadRequest("Villa number is missing");

            _uow.Bookings.UpdateStatus(booking.Id, BookingStatus.Completed, villaNumber);

            await _uow.SaveAsync();

            TempData["success"] = "Checked out successfully";

            return RedirectToAction("BookingDetails", new { bookingId = booking.Id });
        }

        [Authorize(Roles = SD.Role_Admin)]
        [HttpPost]
        public async Task<IActionResult> CancelBooking(int bookingId)
        {
            var booking = await _uow.Bookings.GetAsync(b => b.Id == bookingId);
            if (booking == null)
                return NotFound();
            if (booking.Status == BookingStatus.Cancelled)
                return BadRequest("Booking is already cancelled");
            if (booking.Status == BookingStatus.CheckIn)
                return BadRequest("Cannot cancel a checked-in booking");

            if (booking.Status == BookingStatus.Completed)
                return BadRequest("Cannot cancel a completed booking");
            _uow.Bookings.UpdateStatus(booking.Id, BookingStatus.Cancelled, null);
            await _uow.SaveAsync();
            TempData["success"] = "Booking cancelled successfully";
            return RedirectToAction("BookingDetails", new { bookingId = booking.Id });
        }
    }
}
