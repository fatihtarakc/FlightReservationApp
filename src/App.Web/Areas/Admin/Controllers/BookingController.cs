using App.Web.Controllers;

namespace App.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class BookingController : BaseController
    {
        private readonly IBookingService _bookingService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BookingController(IBookingService bookingService, IHttpContextAccessor httpContextAccessor)
        {
            _bookingService = bookingService;
            _httpContextAccessor = httpContextAccessor;
        }

        private string? Token => TokenHelper.GetToken(_httpContextAccessor);

        public async Task<IActionResult> Index(string? search, int? statusFilter, string? date)
        {
            var result = await _bookingService.GetAllAsync(Token!);
            var bookings = result.IsSuccess ? result.Data ?? new() : new();

            if (!string.IsNullOrWhiteSpace(search))
                bookings = bookings.Where(b =>
                    b.BookingCode.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    b.PassengerName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    b.FlightNumber.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();

            if (statusFilter.HasValue)
                bookings = bookings.Where(b => (int)b.Status == statusFilter.Value).ToList();

            if (!string.IsNullOrWhiteSpace(date) && DateTime.TryParse(date, out var parsedDate))
                bookings = bookings.Where(b => b.DepartureTime.Date == parsedDate.Date).ToList();

            var vm = new AdminBookingListPageVM
            {
                Bookings = bookings,
                Search = search,
                StatusFilter = statusFilter,
                DateFilter = date
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(Guid id)
        {
            var result = await _bookingService.CancelAsync(id, Token!);
            if (!result.IsSuccess)
                NotifyErrorLocalized(result.Message);
            else
                NotifySuccessLocalized(result.Message);
            return RedirectToAction(nameof(Index));
        }
    }
}
