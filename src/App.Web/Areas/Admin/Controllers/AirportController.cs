using App.Web.Controllers;

namespace App.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AirportController : BaseController
    {
        private readonly IAirportService _airportService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AirportController(IAirportService airportService, IHttpContextAccessor httpContextAccessor)
        {
            _airportService = airportService;
            _httpContextAccessor = httpContextAccessor;
        }

        private string? Token => TokenHelper.GetToken(_httpContextAccessor);

        private async Task<AdminAirportFormPageVM> BuildFormPageVm()
        {
            var countries = await _airportService.GetCountriesAsync();
            var timezones = await _airportService.GetTimezonesAsync();
            return new AdminAirportFormPageVM { Countries = countries, Timezones = timezones };
        }

        public async Task<IActionResult> Index(string? search)
        {
            var result = await _airportService.GetAllAsync();
            var airports = result.IsSuccess ? result.Data ?? new() : new();
            if (!string.IsNullOrWhiteSpace(search))
                airports = airports.Where(a =>
                    a.IataCode.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    a.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    a.City.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();

            return View(new AdminAirportListPageVM { Airports = airports, Search = search });
        }

        [HttpGet]
        public async Task<IActionResult> Detail(Guid id)
        {
            var result = await _airportService.GetByIdAsync(id);
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
            var pageVm = await BuildFormPageVm();
            return View(pageVm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AdminAirportFormPageVM model)
        {
            if (!ModelState.IsValid)
            {
                NotifyValidationErrors();
                var filled = await BuildFormPageVm();
                filled.Form = model.Form;
                return View(filled);
            }
            var result = await _airportService.AddAsync(model.Form, Token!);
            if (!result.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                NotifyErrorLocalized(result.Message);
                var filled = await BuildFormPageVm();
                filled.Form = model.Form;
                return View(filled);
            }
            NotifySuccessLocalized(result.Message);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var result = await _airportService.GetByIdAsync(id);
            if (!result.IsSuccess || result.Data == null)
            {
                NotifyErrorLocalized(result.Message);
                return RedirectToAction(nameof(Index));
            }
            var vm = result.Data;
            var pageVm = await BuildFormPageVm();
            pageVm.EditId = id;
            pageVm.Form = new AirportAddVM
            {
                Name      = vm.Name,
                IataCode  = vm.IataCode,
                IcaoCode  = vm.IcaoCode,
                City      = vm.City,
                Country   = vm.Country,
                Timezone  = vm.Timezone,
                Latitude  = vm.Latitude,
                Longitude = vm.Longitude
            };
            return View(pageVm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, AdminAirportFormPageVM model)
        {
            if (!ModelState.IsValid)
            {
                NotifyValidationErrors();
                var filled = await BuildFormPageVm();
                filled.EditId = id;
                filled.Form = model.Form;
                return View(filled);
            }
            var result = await _airportService.UpdateAsync(id, model.Form, Token!);
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
            var result = await _airportService.DeleteAsync(id, Token!);
            if (!result.IsSuccess)
                NotifyErrorLocalized(result.Message);
            else
                NotifySuccessLocalized(result.Message);
            return RedirectToAction(nameof(Index));
        }
    }
}
