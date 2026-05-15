namespace App.Web.Controllers
{
    public class FlightController : Controller
    {
        private readonly IApiService _apiService;

        public FlightController(IApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpGet]
        public IActionResult Search()
        {
            return View(new FlightSearchViewModel { DepartureDate = DateTime.Today.AddDays(1) });
        }

        [HttpPost]
        public async Task<IActionResult> Search(FlightSearchViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            var qs = $"flight/search?departureIata={Uri.EscapeDataString(viewModel.DepartureIata)}" +
                     $"&arrivalIata={Uri.EscapeDataString(viewModel.ArrivalIata)}" +
                     $"&departureDate={viewModel.DepartureDate:yyyy-MM-dd}" +
                     $"&passengers={viewModel.Passengers}" +
                     $"&seatClass={(int)viewModel.SeatClass}";

            var token = HttpContext.Session.GetString("jwt_token");
            var response = await _apiService.GetAsync<List<FlightListDto>>(qs, token);

            if (response?.Success == true && response.Data != null)
                viewModel.Results = response.Data;
            else
                TempData["ErrorMessage"] = response?.Message ?? "No flights found for this route.";

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var token = HttpContext.Session.GetString("jwt_token");
            var response = await _apiService.GetAsync<FlightDto>($"flight/{id}", token);

            if (response?.Success != true || response.Data == null)
            {
                TempData["ErrorMessage"] = "Flight not found.";
                return RedirectToAction(nameof(Search));
            }

            return View(response.Data);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Book(Guid flightId)
        {
            var token = HttpContext.Session.GetString("jwt_token");

            var flightResponse = await _apiService.GetAsync<FlightDto>($"flight/{flightId}", token);
            if (flightResponse?.Success != true || flightResponse.Data == null)
            {
                TempData["ErrorMessage"] = "Flight not found.";
                return RedirectToAction(nameof(Search));
            }

            var seatsResponse = await _apiService.GetAsync<List<SeatDto>>($"seat/available/{flightId}", token);

            var viewModel = new BookViewModel
            {
                Flight = flightResponse.Data,
                AvailableSeats = seatsResponse?.Data ?? new List<SeatDto>()
            };

            return View(viewModel);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Book(BookViewModel viewModel)
        {
            var token = HttpContext.Session.GetString("jwt_token");

            var bookingDto = new BookingAddDto
            {
                FlightId = viewModel.Flight.Id,
                SeatId = viewModel.SelectedSeatId
            };

            var response = await _apiService.PostAsync<BookingDto>("bookings", bookingDto, token);

            if (response?.Success != true || response.Data == null)
            {
                TempData["ErrorMessage"] = response?.Message ?? "Booking failed.";
                return RedirectToAction(nameof(Book), new { flightId = viewModel.Flight.Id });
            }

            TempData["SuccessMessage"] = $"Booking confirmed! Your PNR: {response.Data.PnrNumber}";
            return RedirectToAction("Details", "Booking", new { id = response.Data.Id });
        }
    }
}
