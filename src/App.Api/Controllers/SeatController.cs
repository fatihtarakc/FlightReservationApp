namespace App.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SeatController : ControllerBase
    {
        private readonly ISeatService _seatService;

        public SeatController(ISeatService seatService)
        {
            _seatService = seatService;
        }

        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _seatService.GetByIdAsync(id);
            return result.IsSuccess ? Ok(result) : NotFound(result);
        }

        [HttpGet("by-aircraft/{aircraftId:guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByAircraftId(Guid aircraftId)
        {
            var result = await _seatService.GetByAircraftIdAsync(aircraftId);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("available/{flightId:guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAvailableByFlightId(Guid flightId)
        {
            var result = await _seatService.GetAvailableByFlightIdAsync(flightId);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> Add([FromBody] SeatAddDto dto)
        {
            var result = await _seatService.AddAsync(dto);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("bulk")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> AddRange([FromBody] IEnumerable<SeatAddDto> dtos)
        {
            var result = await _seatService.AddRangeAsync(dtos);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _seatService.DeleteAsync(id);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}

