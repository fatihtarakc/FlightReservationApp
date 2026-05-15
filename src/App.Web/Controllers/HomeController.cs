using System.Diagnostics;
using App.Web.Models;

namespace App.Web.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly IFlightService _flightService;
        private readonly IRouteService _routeService;

        public HomeController(IFlightService flightService, IRouteService routeService)
        {
            _flightService = flightService;
            _routeService = routeService;
        }

        public async Task<IActionResult> Index()
        {
            var flightsResult = await _flightService.GetAllAsync();
            var routesResult = await _routeService.GetAllAsync();
            var vm = new HomePageVM
            {
                RecentFlights = flightsResult.IsSuccess ? flightsResult.Data?.Take(6).ToList() ?? new() : new(),
                PopularRoutes = routesResult.IsSuccess ? routesResult.Data?.Take(6).ToList() ?? new() : new()
            };
            return View(vm);
        }

        public IActionResult About() => View();

        public IActionResult SetLanguage(string lang, string returnUrl)
        {
            HttpContext.Session.SetString(Helpers.SessionRequestCultureProvider.SessionKey, lang);
            return LocalRedirect(string.IsNullOrEmpty(returnUrl) ? "/" : returnUrl);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() =>
            View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
