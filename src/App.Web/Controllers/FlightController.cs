namespace App.Web.Controllers
{
    [AllowAnonymous]
    public class FlightController : BaseController
    {
        private readonly IFlightService _flightService;
        private readonly IAirportService _airportService;
        private readonly ISeatService _seatService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public FlightController(IFlightService flightService, IAirportService airportService,
            ISeatService seatService, IHttpContextAccessor httpContextAccessor)
        {
            _flightService = flightService;
            _airportService = airportService;
            _seatService = seatService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public async Task<IActionResult> Index(FlightSearchVM? search)
        {
            var airports = await _airportService.GetAllAsync();
            var vm = new FlightListPageVM
            {
                SearchVM = search ?? new(),
                Airports = airports.IsSuccess ? airports.Data ?? new() : new(),
                IsLoggedIn = !string.IsNullOrEmpty(TokenHelper.GetToken(_httpContextAccessor))
            };

            if (search != null && !string.IsNullOrWhiteSpace(search.DepartureIata))
            {
                var result = await _flightService.SearchAsync(search);
                if (result.IsSuccess)
                {
                    vm.Flights = result.Data ?? new();
                    vm.IsSearchResult = true;
                }
                else
                {
                    NotifyErrorLocalized(result.Message);
                }
            }
            else
            {
                var result = await _flightService.GetAllAsync();
                if (result.IsSuccess && result.Data != null)
                {
                    vm.Flights = result.Data
                        .Where(f => f.DepartureTime.Date == DateTime.Today && f.DepartureTime >= DateTime.Now)
                        .OrderBy(f => f.DepartureTime)
                        .ToList();
                }
            }

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(Guid id)
        {
            var flightResult = await _flightService.GetByIdAsync(id);
            if (!flightResult.IsSuccess || flightResult.Data == null)
            {
                NotifyErrorLocalized(flightResult.Message);
                return RedirectToAction(nameof(Index));
            }

            var seatsResult = await _seatService.GetByFlightIdAsync(id);
            var vm = new FlightDetailPageVM
            {
                Flight = flightResult.Data,
                Seats = seatsResult.IsSuccess ? seatsResult.Data ?? new() : new(),
                IsLoggedIn = !string.IsNullOrEmpty(TokenHelper.GetToken(_httpContextAccessor))
            };
            return View(vm);
        }
    }
}
