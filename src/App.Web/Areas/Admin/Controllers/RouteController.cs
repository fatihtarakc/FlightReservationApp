using App.Web.Controllers;

namespace App.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
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
                return View(await BuildFormVm(page.Form));

            var result = await _routeService.AddAsync(page.Form, Token!);
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
