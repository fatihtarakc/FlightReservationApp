using App.Web.Controllers;

namespace App.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class AirportController : BaseController
    {
        private readonly IAirportService _airportService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public AirportController(IAirportService airportService, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _airportService = airportService;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        private string? Token => TokenHelper.GetToken(_httpContextAccessor);

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
        public IActionResult Create() => View(new AirportAddVM());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AirportAddVM model)
        {
            if (!ModelState.IsValid) return View(model);
            var result = await _airportService.AddAsync(model, Token!);
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
            var result = await _airportService.GetByIdAsync(id);
            if (!result.IsSuccess || result.Data == null)
            {
                NotifyErrorLocalized(result.Message);
                return RedirectToAction(nameof(Index));
            }
            var model = _mapper.Map<AirportAddVM>(result.Data!);
            ViewBag.EditId = id;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, AirportAddVM model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.EditId = id;
                return View(model);
            }
            var result = await _airportService.UpdateAsync(id, model, Token!);
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
