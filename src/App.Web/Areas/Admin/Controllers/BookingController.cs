namespace App.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class BookingController : Controller
    {
        private readonly IAdminWebService _adminService;

        public BookingController(IAdminWebService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var token = HttpContext.Session.GetString("jwt_token")!;
            var response = await _adminService.GetAllBookingsAsync(token);
            return View(response?.Data ?? new List<AdminBookingListItemViewModel>());
        }
    }
}
