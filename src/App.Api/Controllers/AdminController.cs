namespace App.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "AdminPolicy")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            var result = await _adminService.GetDashboardAsync();
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("flight-passenger-stats")]
        public async Task<IActionResult> GetFlightPassengerStats()
        {
            var result = await _adminService.GetFlightPassengerStatsAsync();
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}
