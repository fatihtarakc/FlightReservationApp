using App.Web.Controllers;

namespace App.Web.Areas.Passenger.Controllers
{
    [Area("Passenger")]
    [Authorize(Roles = "AppUser")]
    public class BookingController : BaseController
    {
        private readonly IBookingService _bookingService;
        private readonly IFlightService _flightService;
        private readonly ISeatService _seatService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BookingController(IBookingService bookingService, IFlightService flightService,
            ISeatService seatService, IHttpContextAccessor httpContextAccessor)
        {
            _bookingService = bookingService;
            _flightService = flightService;
            _seatService = seatService;
            _httpContextAccessor = httpContextAccessor;
        }

        private string? Token => TokenHelper.GetToken(_httpContextAccessor);

        public async Task<IActionResult> MyBookings()
        {
            var token = Token;
            if (string.IsNullOrEmpty(token)) return RedirectToAction("SignIn", "Account", new { area = "" });
            var result = await _bookingService.GetMyBookingsAsync(token);
            return View(result.IsSuccess ? result.Data ?? new() : new List<BookingVM>());
        }

        [HttpGet]
        public async Task<IActionResult> Create(Guid flightId, Guid seatId)
        {
            var token = Token;
            if (string.IsNullOrEmpty(token)) return RedirectToAction("SignIn", "Account", new { area = "" });

            var flightTask = _flightService.GetByIdAsync(flightId);
            var seatTask = _seatService.GetByIdAsync(seatId);
            await Task.WhenAll(flightTask, seatTask);

            var flightResult = await flightTask;
            var seatResult = await seatTask;

            if (!flightResult.IsSuccess || flightResult.Data == null) return NotFound();
            if (!seatResult.IsSuccess || seatResult.Data == null) return NotFound();

            var flight = flightResult.Data;
            var seat = seatResult.Data;

            var price = seat.SeatClass switch
            {
                SeatClass.Economy => flight.EconomyPrice,
                SeatClass.PremiumEconomy => flight.PremiumEconomyPrice ?? flight.EconomyPrice,
                SeatClass.Business => flight.BusinessPrice ?? flight.EconomyPrice,
                SeatClass.First => flight.FirstClassPrice ?? flight.EconomyPrice,
                _ => flight.EconomyPrice
            };

            var vm = new BookingCreatePageVM
            {
                Form = new BookingAddVM { FlightId = flightId, SeatId = seatId },
                Flight = flight,
                Seat = seat,
                TotalPrice = price
            };
            return View(vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookingCreatePageVM model)
        {
            var token = Token;
            if (string.IsNullOrEmpty(token)) return RedirectToAction("SignIn", "Account", new { area = "" });

            var result = await _bookingService.CreateAsync(model.Form, token);
            if (!result.IsSuccess)
            {
                NotifyErrorLocalized(result.Message);
                return RedirectToAction("Create", new { flightId = model.Form.FlightId, seatId = model.Form.SeatId });
            }
            NotifySuccessLocalized(result.Message);
            return RedirectToAction(nameof(MyBookings));
        }

        [HttpGet]
        public async Task<IActionResult> Detail(Guid id)
        {
            var token = Token;
            if (string.IsNullOrEmpty(token)) return RedirectToAction("SignIn", "Account", new { area = "" });

            var result = await _bookingService.GetByIdAsync(id, token);
            if (!result.IsSuccess || result.Data == null) return NotFound();

            var booking = result.Data;
            var canCancel = booking.Status == BookingStatus.Confirmed || booking.Status == BookingStatus.Pending;
            var canCheckIn = booking.Status == BookingStatus.Confirmed && booking.DepartureTime > DateTime.UtcNow && booking.DepartureTime < DateTime.UtcNow.AddHours(24);

            return View(new BookingDetailPageVM { Booking = booking, CanCancel = canCancel, CanCheckIn = canCheckIn });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(Guid id)
        {
            var result = await _bookingService.CancelAsync(id, Token!);
            if (!result.IsSuccess)
                NotifyErrorLocalized(result.Message);
            else
                NotifySuccessLocalized(result.Message);
            return RedirectToAction(nameof(MyBookings));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckIn(Guid id)
        {
            var result = await _bookingService.CheckInAsync(id, Token!);
            if (!result.IsSuccess)
                NotifyErrorLocalized(result.Message);
            else
                NotifySuccessLocalized(result.Message);
            return RedirectToAction(nameof(MyBookings));
        }
    }
}
