using App.Web.Controllers;

namespace App.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ScheduleController : BaseController
    {
        private readonly IScheduleService _scheduleService;
        private readonly IRouteService _routeService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ScheduleController(IScheduleService scheduleService, IRouteService routeService,
            IHttpContextAccessor httpContextAccessor)
        {
            _scheduleService = scheduleService;
            _routeService = routeService;
            _httpContextAccessor = httpContextAccessor;
        }

        private string? Token => TokenHelper.GetToken(_httpContextAccessor);

        public async Task<IActionResult> Index(string? search)
        {
            var result = await _scheduleService.GetAllFullAsync();
            var schedules = result.IsSuccess ? result.Data ?? new() : new List<ScheduleVM>();

            if (!string.IsNullOrEmpty(search))
                schedules = schedules
                    .Where(s => s.Code.Contains(search, StringComparison.OrdinalIgnoreCase)
                        || s.DepartureAirport.Contains(search, StringComparison.OrdinalIgnoreCase)
                        || s.ArrivalAirport.Contains(search, StringComparison.OrdinalIgnoreCase))
                    .ToList();

            return View(new AdminScheduleListPageVM { Schedules = schedules, Search = search });
        }

        [HttpGet]
        public async Task<IActionResult> Detail(Guid id)
        {
            var result = await _scheduleService.GetByIdAsync(id);
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
            return View(await BuildFormPageVm());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AdminScheduleFormPageVM model)
        {
            if (!ModelState.IsValid)
            {
                NotifyValidationErrors();
                var filled = await BuildFormPageVm();
                filled.Form = model.Form;
                return View(filled);
            }
            var result = await _scheduleService.AddAsync(model.Form, Token!);
            if (!result.IsSuccess)
            {
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
            var result = await _scheduleService.GetByIdAsync(id);
            if (!result.IsSuccess || result.Data == null)
            {
                NotifyErrorLocalized(result.Message);
                return RedirectToAction(nameof(Index));
            }
            var s = result.Data;
            return View(new AdminScheduleEditPageVM
            {
                EditId    = id,
                RouteName = s.RouteName,
                Form      = new ScheduleUpdateVM
                {
                    ValidFrom     = s.ValidFrom,
                    ValidTo       = s.ValidTo,
                    SelectedDays  = ExtractDays(s.DaysOfWeek),
                    DepartureTime = s.DepartureTime.ToString(@"HH\:mm"),
                    TimeZone      = s.TimeZone
                }
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, AdminScheduleEditPageVM model)
        {
            if (!ModelState.IsValid)
            {
                NotifyValidationErrors();
                return View(model);
            }
            var result = await _scheduleService.UpdateAsync(id, model.Form, Token!);
            if (!result.IsSuccess)
            {
                NotifyErrorLocalized(result.Message);
                return View(model);
            }
            NotifySuccessLocalized(result.Message);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> HasFlights(Guid id)
        {
            var hasFlights = await _scheduleService.HasFlightsAsync(id);
            return Json(new { hasFlights });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (await _scheduleService.HasFlightsAsync(id))
            {
                NotifyErrorLocalized(Messages.Schedule_HasFlights_Error);
                return RedirectToAction(nameof(Index));
            }
            var result = await _scheduleService.DeleteAsync(id, Token!);
            if (!result.IsSuccess)
                NotifyErrorLocalized(result.Message);
            else
                NotifySuccessLocalized(result.Message);
            return RedirectToAction(nameof(Index));
        }

        private async Task<AdminScheduleFormPageVM> BuildFormPageVm()
        {
            var routeResult = await _routeService.GetAllAsync();
            return new AdminScheduleFormPageVM
            {
                Routes = routeResult.IsSuccess ? routeResult.Data ?? new() : new()
            };
        }

        private static List<int> ExtractDays(DaysOfWeek days)
        {
            var selected = new List<int>();
            foreach (var day in new[] { DaysOfWeek.Monday, DaysOfWeek.Tuesday, DaysOfWeek.Wednesday,
                DaysOfWeek.Thursday, DaysOfWeek.Friday, DaysOfWeek.Saturday, DaysOfWeek.Sunday })
            {
                if (days.HasFlag(day)) selected.Add((int)day);
            }
            return selected;
        }
    }
}
