namespace App.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private readonly IAppUserService _appUserService;

        public BookingController(IBookingService bookingService, IAppUserService appUserService)
        {
            _bookingService = bookingService;
            _appUserService = appUserService;
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _bookingService.GetByIdAsync(id);
            return result.IsSuccess ? Ok(result) : NotFound(result);
        }

        [HttpGet("pnr/{pnr}")]
        public async Task<IActionResult> GetByPnr(string pnr)
        {
            var result = await _bookingService.GetByPnrAsync(pnr);
            return result.IsSuccess ? Ok(result) : NotFound(result);
        }

        [HttpGet("my-bookings")]
        public async Task<IActionResult> GetMyBookings()
        {
            var identityId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var userResult = await _appUserService.GetByIdentityIdAsync(identityId);
            if (!userResult.IsSuccess)
                return Unauthorized();

            var result = await _bookingService.GetByUserIdAsync(userResult.Data!.Id);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("by-flight/{flightId:guid}")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> GetByFlightId(Guid flightId)
        {
            var result = await _bookingService.GetByFlightIdAsync(flightId);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] BookingAddDto dto)
        {
            var identityId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var userResult = await _appUserService.GetByIdentityIdAsync(identityId);
            if (!userResult.IsSuccess)
                return Unauthorized();

            var result = await _bookingService.AddAsync(userResult.Data!.Id, dto);
            return result.IsSuccess ? CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result) : BadRequest(result);
        }

        [HttpPost("{id:guid}/cancel")]
        public async Task<IActionResult> Cancel(Guid id, [FromQuery] string? reason)
        {
            var result = await _bookingService.CancelAsync(id, reason);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}

