using Portfolio.API.Authorization;
using Portfolio.API.Models;

namespace Portfolio.API.Controllers;

[ApiController]
[Route("api/v1/projects")]
[Route("api/[controller]")]
[Produces("application/json")]
public sealed class ProjectsController : ControllerBase
{
    private readonly IUnitOfWork _unit;

    public ProjectsController(IUnitOfWork unit) => _unit = unit;

    [HttpGet]
    [HttpGet("GetAll")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<Project>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Project>>> GetAll()
    {
        var projects = await _unit.Projects.GetAllAsync().ConfigureAwait(false);
        return Ok(projects);
    }

    [HttpGet("{id:int}")]
    [HttpGet("GetById/{id:int}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(Project), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Project>> GetById(int id)
    {
        var project = await _unit.Projects.GetByIdAsync(id).ConfigureAwait(false);
        if (project is null)
            return NotFound($"Project with id {id} was not found.");
        return Ok(project);
    }

    [HttpGet("count")]
    [AllowAnonymous]
    public async Task<ActionResult<int>> Count()
    {
        var count = await _unit.Projects.CountAsync().ConfigureAwait(false);
        return Ok(count);
    }

    [HttpGet("paged")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ResultPagesDto<Project>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ResultPagesDto<Project>>> GetPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 5)
    {
        if (page <= 0 || pageSize <= 0)
            return BadRequest("page and pageSize must be greater than 0.");

        var items = await _unit.Projects.GetAllAsync(page, pageSize).ConfigureAwait(false);
        var totalCount = await _unit.Projects.CountAsync().ConfigureAwait(false);
        return Ok(new ResultPagesDto<Project>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        });
    }

    [HttpGet("featured")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<Project>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Project>>> GetFeatured([FromQuery] int take = 6)
    {
        if (take <= 0 || take > 24)
            return BadRequest("take must be between 1 and 24.");

        var projects = await _unit.Projects.GetAllAsync().ConfigureAwait(false);
        var featured = projects
            .Where(p => p.IsFeatured)
            .Take(take)
            .ToList();

        return Ok(featured);
    }

    [HttpPost]
    [HttpPost("Create")]
    [Authorize(Policy = AppPolicies.AdminAccess)]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromForm] ProjectsDto project)
    {
        await _unit.Projects.AddAsync(project).ConfigureAwait(false);
        await _unit.CompleteAsync().ConfigureAwait(false);
        return StatusCode(StatusCodes.Status201Created);
    }

    [HttpPut("{id:int}")]
    [HttpPut("Update/{id:int}")]
    [Authorize(Policy = AppPolicies.AdminAccess)]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromForm] ProjectsDto project)
    {
        var existing = await _unit.Projects.GetByIdAsync(id).ConfigureAwait(false);
        if (existing is null)
            return NotFound($"Project with id {id} was not found.");

        await _unit.Projects.UpdateAsync(id, project).ConfigureAwait(false);
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
        var project = await _unit.Projects.GetByIdAsync(id).ConfigureAwait(false);
        if (project is null)
            return NotFound($"Project with id {id} was not found.");

        _unit.Projects.Delete(project);
        await _unit.CompleteAsync().ConfigureAwait(false);
        return NoContent();
    }
}
