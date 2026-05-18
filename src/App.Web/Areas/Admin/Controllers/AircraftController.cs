using App.Web.Controllers;

namespace App.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AircraftController : BaseController
    {
        private readonly IAircraftService     _aircraftService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AircraftController(IAircraftService aircraftService, IHttpContextAccessor httpContextAccessor)
        {
            _aircraftService     = aircraftService;
            _httpContextAccessor = httpContextAccessor;
        }

        private string? Token => TokenHelper.GetToken(_httpContextAccessor);

        public async Task<IActionResult> Index()
        {
            var result = await _aircraftService.GetAllAsync();
            return View(new AdminAircraftListPageVM
            {
                Aircraft = result.IsSuccess ? result.Data ?? new List<AircraftVM>() : new List<AircraftVM>()
            });
        }

        [HttpGet]
        public async Task<IActionResult> Detail(Guid id)
        {
            var result = await _aircraftService.GetByIdAsync(id);
            if (!result.IsSuccess || result.Data == null)
            {
                NotifyErrorLocalized(result.Message);
                return RedirectToAction(nameof(Index));
            }
            return View(result.Data);
        }

        [HttpGet]
        public IActionResult Create() => View(new AircraftAddVM());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AircraftAddVM model)
        {
            if (!ModelState.IsValid)
            {
                NotifyValidationErrors();
                return View(model);
            }
            var result = await _aircraftService.AddAsync(model, Token!);
            if (!result.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                NotifyErrorLocalized(result.Message);
                return View(model);
            }
            NotifySuccessLocalized(result.Message);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var result = await _aircraftService.GetByIdAsync(id);
            if (!result.IsSuccess || result.Data == null)
            {
                NotifyErrorLocalized(result.Message);
                return RedirectToAction(nameof(Index));
            }
            var vm = result.Data;
            return View(new AircraftUpdateVM
            {
                TailNumber      = vm.RegistrationNumber,
                ManufactureYear = vm.ManufactureYear,
                AircraftStatus  = vm.Status,
                AirlineId       = vm.AirlineId,
                ModelId         = vm.ModelId
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, AircraftUpdateVM model)
        {
            if (!ModelState.IsValid) { NotifyValidationErrors(); return View(model); }
            var result = await _aircraftService.UpdateAsync(id, model, Token!);
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
            var result = await _aircraftService.DeleteAsync(id, Token!);
            if (!result.IsSuccess)
                NotifyErrorLocalized(result.Message);
            else
                NotifySuccessLocalized(result.Message);
            return RedirectToAction(nameof(Index));
        }
    }
}
