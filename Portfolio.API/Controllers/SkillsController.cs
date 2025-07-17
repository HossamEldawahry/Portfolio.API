using Portfolio.API.Models;

namespace Portfolio.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SkillsController : ControllerBase
    {
        private readonly IUnitOfWork _unit;
        public SkillsController(IUnitOfWork unit)
        {
            _unit = unit;
        }
        [HttpGet("GetAll")]
        public async Task<ActionResult<IEnumerable<Skill>>> GetAll()
        {
            
            var skill = await _unit.Skills.GetAllAsync().ConfigureAwait(true);
            if (skill == null)
            {
                return NotFound($"skill Are Not Exist");
            }
            return Ok(skill);
        }
        [HttpGet()]
        [Route("GetById/{id}")]
        public async Task<ActionResult<Skill>> GetById(int id)
        {
            var skill = await _unit.Skills.GetByIdAsync(id).ConfigureAwait(true);
            if (skill == null)
            {
                return NotFound($"Skill With ID --- {id} --- Not Exist");
            }
            return Ok(skill);
        }
        [HttpGet("count")]
        public async Task<ActionResult<int>> Count()
        {
            var count = await _unit.Skills.CountAsync().ConfigureAwait(true);
            return Ok(count);
        }
        [HttpPost("Create")]
        public async Task<ActionResult<Skill>> Create(Skill skill)
        {
            if (skill == null)
            {
                return BadRequest("Skill cannot be null.");
            }
            await _unit.Skills.AddAsync(skill).ConfigureAwait(true);
            await _unit.CompleteAsync().ConfigureAwait(true);
            return Ok();
        }
        [HttpPut()]
        [Route("Update/{id}")]
        public async Task<IActionResult> Update(int id, Skill skill)
        {
            if (skill == null)
            {
                return BadRequest("Skill cannot be null.");
            }
            await _unit.Skills.UpdateAsync(id, skill).ConfigureAwait(true);
            await _unit.CompleteAsync().ConfigureAwait(true);
            return Ok();
        }
        [HttpDelete()]
        [Route("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var skill = await _unit.Skills.GetByIdAsync(id).ConfigureAwait(true);
            if (skill == null)
            {
                return NotFound($"Skill With ID --- {id} --- Not Exist");
            }
            _unit.Skills.Delete(skill);
            await _unit.CompleteAsync().ConfigureAwait(true);
            return NoContent();
        }
    }
}
