namespace Portfolio.API.Controllers;

[ApiController]
[Route("api/v1/stats")]
[Produces("application/json")]
public sealed class StatsController : ControllerBase
{
    private readonly IUnitOfWork _unit;

    public StatsController(IUnitOfWork unit) => _unit = unit;

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PortfolioStatsDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<PortfolioStatsDto>> Get()
    {
        var projectsCount = await _unit.Projects.CountAsync().ConfigureAwait(false);
        var skillsCount = await _unit.Skills.CountAsync().ConfigureAwait(false);
        var messagesCount = await _unit.Messages.CountAsync().ConfigureAwait(false);

        return Ok(new PortfolioStatsDto
        {
            ProjectsCount = projectsCount,
            SkillsCount = skillsCount,
            MessagesCount = messagesCount
        });
    }
}
