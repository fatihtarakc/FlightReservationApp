namespace App.Web.Controllers
{
    [AllowAnonymous]
    public class RouteController : BaseController
    {
        private readonly IRouteService    _routeService;
        private readonly IFlightService   _flightService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration   _configuration;

        public RouteController(IRouteService routeService, IFlightService flightService,
            IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _routeService      = routeService;
            _flightService     = flightService;
            _httpClientFactory = httpClientFactory;
            _configuration     = configuration;
        }

        public async Task<IActionResult> Popular()
        {
            var routes = await GetSortedRoutesAsync();
            ViewBag.TotalCount = routes.Count;
            return View(routes.Take(6).ToList());
        }

        [HttpGet("api/routes/popular")]
        public async Task<IActionResult> PopularData(int skip = 0, int take = 3)
        {
            var routes = await GetSortedRoutesAsync();
            var page = routes.Skip(skip).Take(take).Select(r => new
            {
                r.OriginIata,
                r.OriginCity,
                r.DestinationIata,
                r.DestinationCity,
                r.DistanceKm,
                r.DurationMinutes,
                r.FlightCount
            }).ToList();

            return Json(new { items = page, total = routes.Count });
        }

        [HttpGet("api/city-image")]
        public async Task<IActionResult> CityImage(string city)
        {
            var apiKey = _configuration["Pexels:ApiKey"];
            if (string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(city))
                return Ok(new { url = (string?)null });

            try
            {
                var client   = _httpClientFactory.CreateClient("PexelsClient");
                var response = await client.GetAsync($"v1/search?query={Uri.EscapeDataString(city)}&per_page=1&orientation=landscape");
                if (!response.IsSuccessStatusCode) return Ok(new { url = (string?)null });

                var json = await response.Content.ReadAsStringAsync();
                using var doc    = System.Text.Json.JsonDocument.Parse(json);
                var photos       = doc.RootElement.GetProperty("photos");
                if (photos.GetArrayLength() == 0) return Ok(new { url = (string?)null });

                var url = photos[0].GetProperty("src").GetProperty("landscape").GetString();
                return Ok(new { url });
            }
            catch
            {
                return Ok(new { url = (string?)null });
            }
        }

        private async Task<List<RouteVM>> GetSortedRoutesAsync()
        {
            var routesTask  = _routeService.GetAllAsync();
            var flightsTask = _flightService.GetAllAsync();
            await Task.WhenAll(routesTask, flightsTask);

            var routes  = routesTask.Result.IsSuccess  ? routesTask.Result.Data  ?? new() : new();
            var flights = flightsTask.Result.IsSuccess ? flightsTask.Result.Data ?? new() : new();

            var flightCounts = flights
                .GroupBy(f => f.OriginIata + "-" + f.DestinationIata)
                .ToDictionary(g => g.Key, g => g.Count());

            foreach (var route in routes)
                route.FlightCount = flightCounts.TryGetValue(route.OriginIata + "-" + route.DestinationIata, out var cnt) ? cnt : 0;

            return routes.OrderByDescending(r => r.FlightCount).ToList();
        }
    }
}
