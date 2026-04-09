using Portfolio.API.Authorization;
using Portfolio.API.Models;

namespace Portfolio.API.Controllers;

[ApiController]
[Route("api/v1/messages")]
[Route("api/Message")]
[Produces("application/json")]
public sealed class MessagesController : ControllerBase
{
    private readonly IUnitOfWork _unit;

    public MessagesController(IUnitOfWork unit) => _unit = unit;

    [HttpGet]
    [HttpGet("GetAll")]
    [Authorize(Policy = AppPolicies.AdminAccess)]
    public async Task<ActionResult<IEnumerable<Message>>> GetAll()
    {
        var messages = await _unit.Messages.GetAllAsync().ConfigureAwait(false);
        return Ok(messages);
    }

    [HttpGet("{id:int}")]
    [HttpGet("GetById/{id:int}")]
    [Authorize(Policy = AppPolicies.AdminAccess)]
    [ProducesResponseType(typeof(Message), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Message>> GetById(int id)
    {
        var message = await _unit.Messages.GetByIdAsync(id).ConfigureAwait(false);
        if (message is null)
            return NotFound($"Message with id {id} was not found.");
        return Ok(message);
    }

    [HttpGet("count")]
    [Authorize(Policy = AppPolicies.AdminAccess)]
    public async Task<ActionResult<int>> Count()
    {
        var count = await _unit.Messages.CountAsync().ConfigureAwait(false);
        return Ok(count);
    }

    [HttpGet("paged")]
    [Authorize(Policy = AppPolicies.AdminAccess)]
    [ProducesResponseType(typeof(ResultPagesDto<Message>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ResultPagesDto<Message>>> GetPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        if (page <= 0 || pageSize <= 0)
            return BadRequest("page and pageSize must be greater than 0.");

        var items = await _unit.Messages.GetAllAsync(page, pageSize).ConfigureAwait(false);
        var totalCount = await _unit.Messages.CountAsync().ConfigureAwait(false);
        return Ok(new ResultPagesDto<Message>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        });
    }

    [HttpPost]
    [HttpPost("Create")]
    [AllowAnonymous]
    [EnableRateLimiting("contact")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> Create([FromBody] MessageCreateDto dto)
    {
        var message = new Message
        {
            Name = dto.Name,
            Email = dto.Email,
            Subject = dto.Subject,
            Content = dto.Content
        };
        await _unit.Messages.AddAsync(message).ConfigureAwait(false);
        await _unit.CompleteAsync().ConfigureAwait(false);
        return StatusCode(StatusCodes.Status201Created);
    }

    [HttpPut("{id:int}")]
    [HttpPut("Update/{id:int}")]
    [Authorize(Policy = AppPolicies.AdminAccess)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] MessageUpdateDto dto)
    {
        var existing = await _unit.Messages.GetByIdAsync(id).ConfigureAwait(false);
        if (existing is null)
            return NotFound($"Message with id {id} was not found.");

        var message = new Message
        {
            Name = dto.Name,
            Email = dto.Email,
            Subject = dto.Subject,
            Content = dto.Content
        };
        await _unit.Messages.UpdateAsync(id, message).ConfigureAwait(false);
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
        var message = await _unit.Messages.GetByIdAsync(id).ConfigureAwait(false);
        if (message is null)
            return NotFound($"Message with id {id} was not found.");

        _unit.Messages.Delete(message);
        await _unit.CompleteAsync().ConfigureAwait(false);
        return NoContent();
    }
}
