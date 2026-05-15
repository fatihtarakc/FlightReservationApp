using App.Web.Controllers;

namespace App.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class FlightController : BaseController
    {
        private readonly IFlightService _flightService;
        private readonly IAircraftService _aircraftService;
        private readonly IRouteService _routeService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public FlightController(IFlightService flightService, IAircraftService aircraftService,
            IRouteService routeService, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _flightService = flightService;
            _aircraftService = aircraftService;
            _routeService = routeService;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        private string? Token => TokenHelper.GetToken(_httpContextAccessor);

        public async Task<IActionResult> Index(string? search, int? status, string? date)
        {
            var result = await _flightService.GetAllAsync();
            var flights = result.IsSuccess ? result.Data ?? new() : new List<FlightVM>();

            if (!string.IsNullOrEmpty(search))
                flights = flights.Where(f => f.FlightNumber.Contains(search, StringComparison.OrdinalIgnoreCase)
                    || f.OriginIata.Contains(search, StringComparison.OrdinalIgnoreCase)
                    || f.DestinationIata.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
            if (status.HasValue)
                flights = flights.Where(f => (int)f.Status == status.Value).ToList();
            if (!string.IsNullOrEmpty(date) && DateTime.TryParse(date, out var d))
                flights = flights.Where(f => f.DepartureTime.Date == d.Date).ToList();

            return View(new AdminFlightListPageVM
            {
                Flights = flights,
                Search = search,
                StatusFilter = status,
                DateFilter = date
            });
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var pageVm = await BuildFormPageVm(null);
            return View(pageVm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AdminFlightFormPageVM model)
        {
            if (!ModelState.IsValid)
            {
                var filled = await BuildFormPageVm(null);
                filled.Form = model.Form;
                return View(filled);
            }
            var result = await _flightService.AddAsync(model.Form, Token!);
            if (!result.IsSuccess)
            {
                NotifyErrorLocalized(result.Message);
                return RedirectToAction(nameof(Index));
            }
            NotifySuccessLocalized(result.Message);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var result = await _flightService.GetByIdAsync(id);
            if (!result.IsSuccess) return NotFound();
            var pageVm = await BuildFormPageVm(id);
            pageVm.Form = _mapper.Map<FlightAddVM>(result.Data!);
            return View(pageVm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, AdminFlightFormPageVM model)
        {
            if (!ModelState.IsValid)
            {
                var filled = await BuildFormPageVm(id);
                filled.Form = model.Form;
                return View(filled);
            }
            var result = await _flightService.UpdateAsync(id, model.Form, Token!);
            if (!result.IsSuccess)
            {
                NotifyErrorLocalized(result.Message);
                return RedirectToAction(nameof(Index));
            }
            NotifySuccessLocalized(result.Message);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(Guid id)
        {
            var result = await _flightService.CancelAsync(id, null, Token!);
            if (!result.IsSuccess)
                NotifyErrorLocalized(result.Message);
            else
                NotifySuccessLocalized(result.Message);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _flightService.DeleteAsync(id, Token!);
            if (!result.IsSuccess)
                NotifyErrorLocalized(result.Message);
            else
                NotifySuccessLocalized(result.Message);
            return RedirectToAction(nameof(Index));
        }

        private async Task<AdminFlightFormPageVM> BuildFormPageVm(Guid? editId)
        {
            var aircraftResult = await _aircraftService.GetAllAsync();
            var routesResult = await _routeService.GetAllAsync();
            return new AdminFlightFormPageVM
            {
                EditId = editId,
                Aircraft = aircraftResult.IsSuccess ? aircraftResult.Data ?? new() : new(),
                Routes = routesResult.IsSuccess ? routesResult.Data ?? new() : new(),
                Form = new FlightAddVM { DepartureTime = DateTime.Now.AddDays(1), ArrivalTime = DateTime.Now.AddDays(1).AddHours(2) }
            };
        }
    }
}
