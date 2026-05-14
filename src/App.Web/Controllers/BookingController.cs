namespace App.Web.Controllers
{
    [Authorize]
    public class BookingController : Controller
    {
        private readonly IApiService _apiService;

        public BookingController(IApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpGet]
        public async Task<IActionResult> MyBookings()
        {
            var token = HttpContext.Session.GetString("jwt_token");
            var response = await _apiService.GetAsync<List<BookingListDto>>("booking/my-bookings", token);

            var bookings = response?.Data ?? new List<BookingListDto>();
            return View(bookings);
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var token = HttpContext.Session.GetString("jwt_token");
            var response = await _apiService.GetAsync<BookingDto>($"booking/{id}", token);

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
            var token = HttpContext.Session.GetString("jwt_token");
            var response = await _apiService.PostAsync<object>($"booking/{id}/cancel", new { }, token);

            if (response?.Success != true)
                TempData["ErrorMessage"] = response?.Message ?? "Cancellation failed.";
            else
                TempData["SuccessMessage"] = "Booking cancelled successfully.";

            return RedirectToAction(nameof(MyBookings));
        }
    }
}
