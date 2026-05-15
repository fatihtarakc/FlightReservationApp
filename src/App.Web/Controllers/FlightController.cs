namespace App.Web.Controllers
{
    public class FlightController : Controller
    {
        private readonly IFlightWebService _flightService;
        private readonly IBookingWebService _bookingService;

        public FlightController(IFlightWebService flightService, IBookingWebService bookingService)
        {
            _flightService = flightService;
            _bookingService = bookingService;
        }

        [HttpGet]
        public IActionResult Search() =>
            View(new FlightSearchViewModel { DepartureDate = DateTime.Today.AddDays(1) });

        [HttpPost]
        public async Task<IActionResult> Search(FlightSearchViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            var token = HttpContext.Session.GetString("jwt_token");
            var response = await _flightService.SearchAsync(
                viewModel.DepartureIata, viewModel.ArrivalIata,
                viewModel.DepartureDate, viewModel.Passengers, (int)viewModel.SeatClass, token);

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
            var response = await _flightService.GetDetailsAsync(id, token);

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

            var flightResponse = await _flightService.GetDetailsAsync(flightId, token);
            if (flightResponse?.Success != true || flightResponse.Data == null)
            {
                TempData["ErrorMessage"] = "Flight not found.";
                return RedirectToAction(nameof(Search));
            }

            var seatsResponse = await _flightService.GetAllSeatsWithAvailabilityAsync(flightId, token);

            return View(new BookFlightViewModel
            {
                Flight = flightResponse.Data,
                AllSeats = seatsResponse?.Data ?? new List<SeatViewModel>()
            });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Book(Guid flightId, Guid selectedSeatId)
        {
            var token = HttpContext.Session.GetString("jwt_token");

            var response = await _bookingService.BookSeatAsync(flightId, selectedSeatId, token!);

            if (response?.Success != true || response.Data == null)
            {
                TempData["ErrorMessage"] = response?.Message ?? "Booking failed.";
                return RedirectToAction(nameof(Book), new { flightId });
            }

            TempData["SuccessMessage"] = $"Booking confirmed! Your PNR: {response.Data.PnrNumber}";
            return RedirectToAction("Details", "Booking", new { id = response.Data.Id });
        }
    }
}
