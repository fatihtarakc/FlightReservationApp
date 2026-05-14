using App.Web.Models.ViewModels;

namespace App.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IApiService _apiService;

        public HomeController(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            var response = await _apiService.GetAsync<List<FlightListDto>>("flight");
            var now = DateTime.UtcNow;
            var upcoming = (response?.Data ?? new List<FlightListDto>())
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
