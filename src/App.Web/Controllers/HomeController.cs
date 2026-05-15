namespace App.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IFlightWebService _flightService;

        public HomeController(IFlightWebService flightService)
        {
            _flightService = flightService;
        }

        public async Task<IActionResult> Index()
        {
            var token = HttpContext.Session.GetString("jwt_token");
            var response = await _flightService.SearchAsync(
                string.Empty, string.Empty, DateTime.Today, 1, 1, token);

            var now = DateTime.UtcNow;
            var upcoming = (response?.Data ?? new List<FlightListItemViewModel>())
                .Where(f => f.DepartureDateTime > now)
                .OrderBy(f => f.DepartureDateTime)
                .Take(8)
                .ToList();

            ViewBag.UpcomingFlights = upcoming;
            return View();
        }

        public IActionResult About() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View();
    }
}
