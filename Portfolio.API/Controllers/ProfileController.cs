using Portfolio.API.Authorization;

namespace Portfolio.API.Controllers;

[ApiController]
[Route("api/v1/profile")]
[Route("api/[controller]")]
[Produces("application/json")]
public sealed class ProfileController : ControllerBase
{
    private readonly IUnitOfWork _unit;

    public ProfileController(IUnitOfWork unit) => _unit = unit;

    [HttpGet("current")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(Profile), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Profile>> GetCurrent()
    {
        var profile = await _unit.Profiles.GetFirstAsync().ConfigureAwait(false);
        if (profile is null)
            return NotFound("No profile has been created yet.");
        return Ok(profile);
    }

    [HttpGet("{id:int}")]
    [HttpGet("GetById/{id:int}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(Profile), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Profile>> GetById(int id)
    {
        var profile = await _unit.Profiles.GetByIdAsync(id).ConfigureAwait(false);
        if (profile is null)
            return NotFound($"Profile with id {id} was not found.");
        return Ok(profile);
    }

    [HttpPost]
    [HttpPost("Create")]
    [Authorize(Policy = AppPolicies.AdminAccess)]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromForm] ProfileDto profile)
    {
        await _unit.Profiles.AddAsync(profile).ConfigureAwait(false);
        await _unit.CompleteAsync().ConfigureAwait(false);
        return StatusCode(StatusCodes.Status201Created);
    }

    [HttpPut("{id:int}")]
    [HttpPut("Update/{id:int}")]
    [Authorize(Policy = AppPolicies.AdminAccess)]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromForm] ProfileDto profile)
    {
        var existing = await _unit.Profiles.GetByIdAsync(id).ConfigureAwait(false);
        if (existing is null)
            return NotFound($"Profile with id {id} was not found.");

        await _unit.Profiles.UpdateAsync(id, profile).ConfigureAwait(false);
        await _unit.CompleteAsync().ConfigureAwait(false);
        return NoContent();
    }
}
