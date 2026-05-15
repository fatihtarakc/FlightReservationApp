namespace App.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly IAdminWebService _adminService;

        public DashboardController(IAdminWebService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var token = HttpContext.Session.GetString("jwt_token")!;
            var dashboardResponse = await _adminService.GetDashboardAsync(token);
            var statsResponse = await _adminService.GetFlightPassengerStatsAsync(token);

            ViewBag.PassengerStats = statsResponse?.Data ?? new List<FlightPassengerStatViewModel>();
            return View(dashboardResponse?.Data ?? new DashboardViewModel());
        }
    }
}
