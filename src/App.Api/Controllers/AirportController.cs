namespace App.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AirportController : ControllerBase
    {
        private readonly IAirportService _airportService;

        public AirportController(IAirportService airportService)
        {
            _airportService = airportService;
        }

        [HttpGet("countries")]
        [AllowAnonymous]
        public async Task<IActionResult> GetDistinctCountries()
        {
            var result = await _airportService.GetDistinctCountriesAsync();
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("timezones")]
        [AllowAnonymous]
        public async Task<IActionResult> GetDistinctTimezones()
        {
            var result = await _airportService.GetDistinctTimezonesAsync();
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var result = await _airportService.GetAllAsync();
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _airportService.GetByIdAsync(id);
            return result.IsSuccess ? Ok(result) : NotFound(result);
        }

        [HttpPost]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> Add([FromBody] AirportAddDto dto)
        {
            var result = await _airportService.AddAsync(dto);
            return result.IsSuccess ? CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result) : BadRequest(result);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> Update(Guid id, [FromBody] AirportUpdateDto dto)
        {
            var result = await _airportService.UpdateAsync(id, dto);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _airportService.DeleteAsync(id);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}

