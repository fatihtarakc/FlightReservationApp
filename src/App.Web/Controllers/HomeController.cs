using System.Diagnostics;
using App.Web.Models;
using App.Web.ViewModels.Flight;
using App.Web.Enums;

namespace App.Web.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly IFlightService  _flightService;
        private readonly IRouteService   _routeService;
        private readonly IAirportService _airportService;
        private readonly ISeatService    _seatService;
        private readonly IAircraftService _aircraftService;

        public HomeController(IFlightService flightService, IRouteService routeService,
            IAirportService airportService, ISeatService seatService, IAircraftService aircraftService)
        {
            _flightService   = flightService;
            _routeService    = routeService;
            _airportService  = airportService;
            _seatService     = seatService;
            _aircraftService = aircraftService;
        }

        public async Task<IActionResult> Index()
        {
            var flightsTask = _flightService.GetAllAsync();
            var routesTask = _routeService.GetAllAsync();
            var airportsTask = _airportService.GetAllAsync();
            await Task.WhenAll(flightsTask, routesTask, airportsTask);
            var vm = new HomePageVM
            {
                RecentFlights = flightsTask.Result.IsSuccess ? flightsTask.Result.Data?.Take(6).ToList() ?? new() : new(),
                PopularRoutes = routesTask.Result.IsSuccess ? routesTask.Result.Data?.Take(6).ToList() ?? new() : new(),
                Airports = airportsTask.Result.IsSuccess ? airportsTask.Result.Data ?? new() : new()
            };
            return View(vm);
        }

        [HttpGet("api/countries")]
        public IActionResult Countries()
        {
            var seen = new HashSet<string>();
            var list = System.Globalization.CultureInfo
                .GetCultures(System.Globalization.CultureTypes.SpecificCultures)
                .Select(c => {
                    try { return new System.Globalization.RegionInfo(c.Name); }
                    catch { return null; }
                })
                .Where(r => r != null && seen.Add(r!.TwoLetterISORegionName))
                .OrderBy(r => r!.DisplayName)
                .Select(r => new { code = r!.TwoLetterISORegionName, name = r!.DisplayName })
                .ToList();
            return Json(list);
        }

        [HttpGet("api/airport-countries")]
        public async Task<IActionResult> AirportCountries()
        {
            var list = await _airportService.GetCountriesAsync();
            return Json(list);
        }

        [HttpGet("api/airport-timezones")]
        public async Task<IActionResult> AirportTimezones()
        {
            var list = await _airportService.GetTimezonesAsync();
            return Json(list);
        }

        [HttpGet("api/aircraft-airlines")]
        public async Task<IActionResult> AircraftAirlines()
        {
            var list = await _aircraftService.GetAirlinesAsync();
            return Json(list);
        }

        [HttpGet("api/aircraft-models")]
        public async Task<IActionResult> AircraftModels()
        {
            var list = await _aircraftService.GetModelsAsync();
            return Json(list);
        }

        [HttpGet("api/airports-list")]
        public async Task<IActionResult> AirportsList()
        {
            var result = await _airportService.GetAllAsync();
            return Json(result.IsSuccess ? result.Data : new List<AirportVM>());
        }

        [HttpGet("api/available-destinations")]
        public async Task<IActionResult> AvailableDestinations(string origin)
        {
            var routesTask = _routeService.GetAllAsync();
            var airportsTask = _airportService.GetAllAsync();
            await Task.WhenAll(routesTask, airportsTask);

            if (!routesTask.Result.IsSuccess || !airportsTask.Result.IsSuccess)
                return Json(new List<AirportVM>());

            var destinationIatas = routesTask.Result.Data!
                .Where(r => r.OriginIata == origin)
                .Select(r => r.DestinationIata)
                .ToHashSet();

            var destinations = airportsTask.Result.Data!
                .Where(a => destinationIatas.Contains(a.IataCode))
                .ToList();

            return Json(destinations);
        }

        [HttpGet("api/flight-dates")]
        public async Task<IActionResult> FlightDates(string origin, string destination)
        {
            var result = await _flightService.GetAllAsync();
            if (!result.IsSuccess || result.Data == null)
                return Json(new List<string>());

            var today = DateTime.Today;
            var dates = result.Data
                .Where(f => f.OriginIata == origin && f.DestinationIata == destination && f.DepartureTime.Date >= today)
                .Select(f => f.DepartureTime.Date.ToString("yyyy-MM-dd"))
                .Distinct()
                .OrderBy(d => d)
                .ToList();

            return Json(dates);
        }

        [HttpGet("api/flight-seats")]
        public async Task<IActionResult> FlightSeats(string origin, string destination, string date)
        {
            if (!DateTime.TryParse(date, out var depDate))
                return Json(new { economy = 9, business = 9 });

            var search = new FlightSearchVM
            {
                DepartureIata = origin,
                ArrivalIata = destination,
                DepartureDate = depDate,
                Passengers = 1,
                SeatClass = SeatClass.Economy
            };

            var flightResult = await _flightService.SearchAsync(search);
            if (!flightResult.IsSuccess || flightResult.Data == null || !flightResult.Data.Any())
                return Json(new { economy = 0, business = 0 });

            var seatTasks = flightResult.Data.Select(f => _seatService.GetByFlightIdAsync(f.Id));
            var allSeats = await Task.WhenAll(seatTasks);

            int eco = 0, biz = 0;
            foreach (var sr in allSeats)
            {
                if (sr.IsSuccess && sr.Data != null)
                {
                    eco += sr.Data.Count(s => s.IsAvailable && s.SeatClass == SeatClass.Economy);
                    biz += sr.Data.Count(s => s.IsAvailable && s.SeatClass == SeatClass.Business);
                }
            }

            return Json(new { economy = eco, business = biz });
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
