
using Portfolio.API.Models;

namespace Portfolio.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly IUnitOfWork _unit ;
        public ProjectsController(IUnitOfWork unit)
        {
            _unit = unit;
        }
        [HttpGet("GetAll")]
        public async Task<ActionResult<IEnumerable<Project>>> GetAll()
        {
            var projects = await _unit.Projects.GetAllAsync().ConfigureAwait(true);
            if (projects == null)
            {
                return NotFound($"Projects Are Not Exist Or Not Finish Yet.");
            }
            return Ok(projects);
        }
        [HttpGet()]
        [Route("GetById/{id}")]
        public async Task<ActionResult<Project>> GetById(int id)
        {
            var project = await _unit.Projects.GetByIdAsync(id).ConfigureAwait(true);
            if (project == null)
            {
                return NotFound($"Project With ID --- {id} --- Not Exist Or Not Finish Yet.");
            }
            return Ok(project);
        }
        [HttpGet("count")]
        public async Task<ActionResult<int>> Count()
        {
            var count = await _unit.Projects.CountAsync().ConfigureAwait(true);
            return Ok(count);
        }
        [HttpPost("Create")]
        public async Task<ActionResult<Project>> Create([FromForm]ProjectsDto project)
        {
            if (project == null)
            {
                return BadRequest("Project cannot be null.");
            }
            await _unit.Projects.AddAsync(project).ConfigureAwait(true);
            await _unit.CompleteAsync().ConfigureAwait(true);
            return Ok();
        }
        [HttpPut()]
        [Route("Update/{id}")]
        public async Task<IActionResult> Update(int id,[FromForm]ProjectsDto project)
        {
            if (project == null)
            {
                return BadRequest("Project cannot be null.");
            }
            var existingProject = await _unit.Projects.GetByIdAsync(id).ConfigureAwait(true);
            if (existingProject == null)
            {
                return NotFound($"Project With ID --- {id} --- Not Exist Or Not Finish Yet.");
            }
            await _unit.Projects.UpdateAsync(id,project).ConfigureAwait(true);
            await _unit.CompleteAsync().ConfigureAwait(true);
            return Ok();
        }
        [HttpDelete()]
        [Route("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var project = await _unit.Projects.GetByIdAsync(id).ConfigureAwait(true);
            if (project == null)
            {
                return NotFound($"Project With ID --- {id} --- Not Exist Or Not Finish Yet.");
            }
            _unit.Projects.Delete(project);
            await _unit.CompleteAsync().ConfigureAwait(true);
            return NoContent();
        }
        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 5)
        {
            if (page <= 0 || pageSize <= 0)
                return BadRequest("page and pageSize must be greater than 0");

            var projects = await _unit.Projects.GetAllAsync(page,pageSize).ConfigureAwait(true);
            var totalCount = await _unit.Projects.CountAsync().ConfigureAwait(true);
            var result = new ResultPagesDto<Project>
            {
                Items = projects,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
            return Ok(result);
        }
    }
}
