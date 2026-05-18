namespace App.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ScheduleController : ControllerBase
    {
        private readonly IScheduleService _scheduleService;

        public ScheduleController(IScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var result = await _scheduleService.GetAllAsync();
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _scheduleService.GetByIdAsync(id);
            return result.IsSuccess ? Ok(result) : NotFound(result);
        }

        [HttpGet("{id:guid}/has-flights")]
        [AllowAnonymous]
        public async Task<IActionResult> HasFlights(Guid id)
        {
            var hasFlights = await _scheduleService.HasFlightsAsync(id);
            return Ok(new { IsSuccess = true, Data = hasFlights });
        }

        [HttpGet("by-route/{routeId:guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByRouteId(Guid routeId)
        {
            var result = await _scheduleService.GetByRouteIdAsync(routeId);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> Add([FromBody] ScheduleAddDto dto)
        {
            var result = await _scheduleService.AddAsync(dto);
            return result.IsSuccess ? CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result) : BadRequest(result);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> Update(Guid id, [FromBody] ScheduleUpdateDto dto)
        {
            var result = await _scheduleService.UpdateAsync(id, dto);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _scheduleService.DeleteAsync(id);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}

