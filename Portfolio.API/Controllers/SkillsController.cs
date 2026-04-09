using Portfolio.API.Authorization;
using Portfolio.API.Models;

namespace Portfolio.API.Controllers;

[ApiController]
[Route("api/v1/skills")]
[Route("api/[controller]")]
[Produces("application/json")]
public sealed class SkillsController : ControllerBase
{
    private readonly IUnitOfWork _unit;

    public SkillsController(IUnitOfWork unit) => _unit = unit;

    [HttpGet]
    [HttpGet("GetAll")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<Skill>>> GetAll()
    {
        var skills = await _unit.Skills.GetAllAsync().ConfigureAwait(false);
        return Ok(skills);
    }

    [HttpGet("{id:int}")]
    [HttpGet("GetById/{id:int}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(Skill), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Skill>> GetById(int id)
    {
        var skill = await _unit.Skills.GetByIdAsync(id).ConfigureAwait(false);
        if (skill is null)
            return NotFound($"Skill with id {id} was not found.");
        return Ok(skill);
    }

    [HttpGet("count")]
    [AllowAnonymous]
    public async Task<ActionResult<int>> Count()
    {
        var count = await _unit.Skills.CountAsync().ConfigureAwait(false);
        return Ok(count);
    }

    [HttpGet("paged")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ResultPagesDto<Skill>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ResultPagesDto<Skill>>> GetPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        if (page <= 0 || pageSize <= 0)
            return BadRequest("page and pageSize must be greater than 0.");

        var items = await _unit.Skills.GetAllAsync(page, pageSize).ConfigureAwait(false);
        var totalCount = await _unit.Skills.CountAsync().ConfigureAwait(false);
        return Ok(new ResultPagesDto<Skill>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        });
    }

    [HttpPost]
    [HttpPost("Create")]
    [Authorize(Policy = AppPolicies.AdminAccess)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] SkillWriteDto dto)
    {
        var skill = new Skill { Name = dto.Name, Level = dto.Level };
        await _unit.Skills.AddAsync(skill).ConfigureAwait(false);
        await _unit.CompleteAsync().ConfigureAwait(false);
        return StatusCode(StatusCodes.Status201Created);
    }

    [HttpPut("{id:int}")]
    [HttpPut("Update/{id:int}")]
    [Authorize(Policy = AppPolicies.AdminAccess)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] SkillWriteDto dto)
    {
        var existing = await _unit.Skills.GetByIdAsync(id).ConfigureAwait(false);
        if (existing is null)
            return NotFound($"Skill with id {id} was not found.");

        var skill = new Skill { Name = dto.Name, Level = dto.Level };
        await _unit.Skills.UpdateAsync(id, skill).ConfigureAwait(false);
        await _unit.CompleteAsync().ConfigureAwait(false);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [HttpDelete("Delete/{id:int}")]
    [Authorize(Policy = AppPolicies.AdminAccess)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var skill = await _unit.Skills.GetByIdAsync(id).ConfigureAwait(false);
        if (skill is null)
            return NotFound($"Skill with id {id} was not found.");

        _unit.Skills.Delete(skill);
        await _unit.CompleteAsync().ConfigureAwait(false);
        return NoContent();
    }
}
