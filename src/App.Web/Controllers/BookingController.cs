namespace App.Web.Controllers
{
    [Authorize]
    public class BookingController : Controller
    {
        private readonly IBookingWebService _bookingService;

        public BookingController(IBookingWebService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpGet]
        public async Task<IActionResult> MyBookings()
        {
            var token = HttpContext.Session.GetString("jwt_token")!;
            var response = await _bookingService.GetMyBookingsAsync(token);
            return View(response?.Data ?? new List<BookingListItemViewModel>());
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var token = HttpContext.Session.GetString("jwt_token")!;
            var response = await _bookingService.GetDetailsAsync(id, token);

            if (response?.Success != true || response.Data == null)
            {
                TempData["ErrorMessage"] = "Booking not found.";
                return RedirectToAction(nameof(MyBookings));
            }

            return View(response.Data);
        }

        [HttpPost]
        public async Task<IActionResult> Cancel(Guid id)
        {
            var token = HttpContext.Session.GetString("jwt_token")!;
            var response = await _bookingService.CancelAsync(id, token);

            if (response?.Success != true)
                TempData["ErrorMessage"] = response?.Message ?? "Cancellation failed.";
            else
                TempData["SuccessMessage"] = "Booking cancelled successfully.";

            return RedirectToAction(nameof(MyBookings));
        }
    }
}
