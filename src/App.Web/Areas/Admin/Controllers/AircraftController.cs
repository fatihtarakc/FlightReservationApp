using App.Web.Controllers;

namespace App.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AircraftController : BaseController
    {
        private readonly IAircraftService _aircraftService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public AircraftController(IAircraftService aircraftService, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _aircraftService = aircraftService;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        private string? Token => TokenHelper.GetToken(_httpContextAccessor);

        public async Task<IActionResult> Index()
        {
            var result = await _aircraftService.GetAllAsync();
            var vm = new AdminAircraftListPageVM
            {
                Aircraft = result.IsSuccess ? result.Data ?? new() : new()
            };
            return View(vm);
        }

        [HttpGet]
        public IActionResult Create() => View(new AircraftAddVM());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AircraftAddVM model)
        {
            if (!ModelState.IsValid) return View(model);
            var result = await _aircraftService.AddAsync(model, Token!);
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
            var result = await _aircraftService.GetByIdAsync(id);
            if (!result.IsSuccess || result.Data == null)
            {
                NotifyErrorLocalized(result.Message);
                return RedirectToAction(nameof(Index));
            }
            var model = _mapper.Map<AircraftAddVM>(result.Data!);
            ViewBag.EditId = id;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, AircraftAddVM model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.EditId = id;
                return View(model);
            }
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
