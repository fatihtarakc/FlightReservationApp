namespace App.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AppUserController : ControllerBase
    {
        private readonly IAppUserService _appUserService;

        public AppUserController(IAppUserService appUserService)
        {
            _appUserService = appUserService;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            var identityId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _appUserService.GetByIdentityIdAsync(identityId);
            return result.IsSuccess ? Ok(result) : NotFound(result);
        }

        [HttpGet("{id:guid}")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _appUserService.GetByIdAsync(id);
            return result.IsSuccess ? Ok(result) : NotFound(result);
        }

        [HttpGet]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _appUserService.GetAllAsync();
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] AppUserDto dto)
        {
            var result = await _appUserService.UpdateAsync(id, dto);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("{id:guid}/status")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> SetStatus(Guid id, [FromBody] UserStatusDto dto)
        {
            var result = await _appUserService.SetStatusAsync(id, dto.IsActive);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("{id:guid}/confirm-email")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> ConfirmEmail(Guid id)
        {
            var result = await _appUserService.ConfirmEmailAsync(id);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}

