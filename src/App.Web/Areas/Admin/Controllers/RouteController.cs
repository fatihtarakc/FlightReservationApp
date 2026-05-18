using App.Web.Controllers;

namespace App.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class RouteController : BaseController
    {
        private readonly IRouteService _routeService;
        private readonly IAirportService _airportService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RouteController(IRouteService routeService, IAirportService airportService, IHttpContextAccessor httpContextAccessor)
        {
            _routeService = routeService;
            _airportService = airportService;
            _httpContextAccessor = httpContextAccessor;
        }

        private string? Token => TokenHelper.GetToken(_httpContextAccessor);

        public async Task<IActionResult> Index()
        {
            var result = await _routeService.GetAllAsync();
            var vm = new AdminRouteListPageVM
            {
                Routes = result.IsSuccess ? result.Data ?? new() : new()
            };
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var vm = await BuildFormVm(new RouteAddVM());
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AdminRouteFormPageVM page)
        {
            if (!ModelState.IsValid)
            {
                NotifyValidationErrors();
                return View(await BuildFormVm(page.Form));
            }

            var result = await _routeService.AddAsync(page.Form, Token!);
            if (!result.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View(await BuildFormVm(page.Form));
            }
            NotifySuccessLocalized(result.Message);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> GetTakenDestinations(Guid departureAirportId)
        {
            var routesResult   = await _routeService.GetAllAsync();
            var airportsResult = await _airportService.GetAllAsync();

            var routes   = routesResult.IsSuccess   ? routesResult.Data   ?? new() : new();
            var airports = airportsResult.IsSuccess ? airportsResult.Data ?? new() : new();

            var departure = airports.FirstOrDefault(a => a.Id == departureAirportId);
            if (departure == null)
                return Json(Array.Empty<Guid>());

            var takenIatas = routes
                .Where(r => r.OriginIata == departure.IataCode)
                .Select(r => r.DestinationIata)
                .ToHashSet();

            var takenIds = airports
                .Where(a => takenIatas.Contains(a.IataCode))
                .Select(a => a.Id)
                .ToList();

            return Json(takenIds);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(Guid id)
        {
            var routeResult = await _routeService.GetByIdAsync(id);
            if (!routeResult.IsSuccess || routeResult.Data == null)
            {
                NotifyErrorLocalized(routeResult.Message);
                return RedirectToAction(nameof(Index));
            }
            var r = routeResult.Data;
            var airportsResult = await _airportService.GetAllAsync();
            var airports = airportsResult.IsSuccess ? airportsResult.Data ?? new() : new();
            return View(new AdminRouteDetailPageVM
            {
                Route = r,
                Origin = airports.FirstOrDefault(a => a.IataCode == r.OriginIata),
                Destination = airports.FirstOrDefault(a => a.IataCode == r.DestinationIata)
            });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var routeResult = await _routeService.GetByIdAsync(id);
            if (!routeResult.IsSuccess)
            {
                NotifyErrorLocalized(routeResult.Message);
                return RedirectToAction(nameof(Index));
            }
            var r = routeResult.Data!;
            var airportsResult = await _airportService.GetAllAsync();
            var airports = airportsResult.IsSuccess ? airportsResult.Data ?? new() : new();
            var origin = airports.FirstOrDefault(a => a.IataCode == r.OriginIata);
            var destination = airports.FirstOrDefault(a => a.IataCode == r.DestinationIata);
            var vm = new AdminRouteEditPageVM
            {
                EditId = id,
                Form = new RouteUpdateVM
                {
                    OriginAirportId = origin?.Id ?? Guid.Empty,
                    DestinationAirportId = destination?.Id ?? Guid.Empty,
                    DistanceKm = r.DistanceKm,
                    EstimatedDuration = r.DurationMinutes
                },
                Airports = airports
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, AdminRouteEditPageVM page)
        {
            if (!ModelState.IsValid)
            {
                NotifyValidationErrors();
                var airports = await _airportService.GetAllAsync();
                page.EditId = id;
                page.Airports = airports.IsSuccess ? airports.Data ?? new() : new();
                return View(page);
            }
            var result = await _routeService.UpdateAsync(id, page.Form, Token!);
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
            var result = await _routeService.DeleteAsync(id, Token!);
            if (!result.IsSuccess)
                NotifyErrorLocalized(result.Message);
            else
                NotifySuccessLocalized(result.Message);
            return RedirectToAction(nameof(Index));
        }

        private async Task<AdminRouteFormPageVM> BuildFormVm(RouteAddVM form)
        {
            var airports = await _airportService.GetAllAsync();
            return new AdminRouteFormPageVM
            {
                Form = form,
                Airports = airports.IsSuccess ? airports.Data ?? new() : new()
            };
        }
    }
}
