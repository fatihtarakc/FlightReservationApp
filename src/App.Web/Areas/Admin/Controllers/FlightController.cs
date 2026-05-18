using App.Web.Controllers;
using Microsoft.Extensions.Localization;

namespace App.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class FlightController : BaseController
    {
        private readonly IFlightService _flightService;
        private readonly IAircraftService _aircraftService;
        private readonly IScheduleService _scheduleService;
        private readonly IRouteService _routeService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<SharedResources> _localizer;

        public FlightController(IFlightService flightService, IAircraftService aircraftService,
            IScheduleService scheduleService, IRouteService routeService,
            IHttpContextAccessor httpContextAccessor, IMapper mapper,
            IStringLocalizer<SharedResources> localizer)
        {
            _flightService = flightService;
            _aircraftService = aircraftService;
            _scheduleService = scheduleService;
            _routeService = routeService;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _localizer = localizer;
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
        public async Task<IActionResult> Detail(Guid id)
        {
            var result = await _flightService.GetByIdAsync(id);
            if (!result.IsSuccess || result.Data == null)
            {
                NotifyErrorLocalized(result.Message);
                return RedirectToAction(nameof(Index));
            }
            return View(result.Data);
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
                NotifyValidationErrors();
                var filled = await BuildFormPageVm(null);
                filled.Form = model.Form;
                return View(filled);
            }
            var result = await _flightService.AddAsync(model.Form, Token!);
            if (!result.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                NotifyErrorLocalized(result.Message);
                var filled = await BuildFormPageVm(null);
                filled.Form = model.Form;
                return View(filled);
            }
            NotifySuccessLocalized(result.Message);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var result = await _flightService.GetByIdAsync(id);
            if (!result.IsSuccess) return NotFound();
            if (result.Data!.Status == FlightStatus.Arrived)
            {
                NotifyWarning(_localizer["Flight_ArrivedNoAction"]);
                return RedirectToAction(nameof(Index));
            }
            var pageVm = await BuildFormPageVm(id);
            var f = result.Data!;
            pageVm.Form = new FlightAddVM
            {
                FlightNumber        = f.FlightNumber,
                DepartureTime       = f.DepartureTime,
                ArrivalTime         = f.ArrivalTime,
                EconomyPrice        = f.EconomyPrice,
                PremiumEconomyPrice = f.PremiumEconomyPrice,
                BusinessPrice       = f.BusinessPrice,
                FirstClassPrice     = f.FirstClassPrice
            };
            return View(pageVm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, AdminFlightFormPageVM model)
        {
            var flight = await _flightService.GetByIdAsync(id);
            if (flight.IsSuccess && flight.Data?.Status == FlightStatus.Arrived)
            {
                NotifyWarning(_localizer["Flight_ArrivedNoAction"]);
                return RedirectToAction(nameof(Index));
            }
            if (!ModelState.IsValid)
            {
                NotifyValidationErrors();
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
            var flight = await _flightService.GetByIdAsync(id);
            if (flight.IsSuccess && flight.Data?.Status == FlightStatus.Arrived)
            {
                NotifyWarning(_localizer["Flight_ArrivedNoAction"]);
                return RedirectToAction(nameof(Index));
            }
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
            var flight = await _flightService.GetByIdAsync(id);
            if (flight.IsSuccess && flight.Data?.Status == FlightStatus.Arrived)
            {
                NotifyWarning(_localizer["Flight_ArrivedNoAction"]);
                return RedirectToAction(nameof(Index));
            }
            var result = await _flightService.DeleteAsync(id, Token!);
            if (!result.IsSuccess)
                NotifyErrorLocalized(result.Message);
            else
                NotifySuccessLocalized(result.Message);
            return RedirectToAction(nameof(Index));
        }

        private async Task<AdminFlightFormPageVM> BuildFormPageVm(Guid? editId)
        {
            var aircraftTask  = _aircraftService.GetAllAsync();
            var scheduleTask  = _scheduleService.GetAllAsync();
            var routeTask     = _routeService.GetAllAsync();
            await Task.WhenAll(aircraftTask, scheduleTask, routeTask);

            var aircraft  = (await aircraftTask).IsSuccess ? (await aircraftTask).Data ?? new() : new();
            var schedules = await scheduleTask;
            var routes    = (await routeTask).IsSuccess ? (await routeTask).Data ?? new() : new();

            var durationMap = routes.ToDictionary(r => r.Id, r => r.DurationMinutes);
            foreach (var s in schedules)
                if (durationMap.TryGetValue(s.RouteId, out var dur))
                    s.DurationMinutes = dur;

            return new AdminFlightFormPageVM
            {
                EditId    = editId,
                Aircraft  = aircraft,
                Schedules = schedules,
                Form      = new FlightAddVM { DepartureTime = DateTime.Today.AddDays(1).AddHours(9), ArrivalTime = DateTime.Today.AddDays(1).AddHours(11) }
            };
        }
    }
}
