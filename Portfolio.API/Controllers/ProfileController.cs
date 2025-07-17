using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Portfolio.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly IUnitOfWork _unit;
        public ProfileController(IUnitOfWork unit)
        {
            _unit = unit;
        }
        [HttpGet()]
        [Route("GetById/{id}")]
        public async Task<ActionResult<Profile>> GetById(int id)
        {
            var profile = await _unit.Profiles.GetByIdAsync(id).ConfigureAwait(true);
            if (profile == null)
            {
                return NotFound($"Profile With ID --- {id} --- Not Exist Or Not Finish Yet.");
            }
            return Ok(profile);
        }
        [HttpPost("Create")]
        public async Task<ActionResult<Profile>> Create([FromForm] ProfileDto profile)
        {
            if (profile == null)
            {
                return BadRequest("Profile cannot be null.");
            }
            await _unit.Profiles.AddAsync(profile).ConfigureAwait(true);
            await _unit.CompleteAsync().ConfigureAwait(true);
            return Ok();
        }
        [HttpPut()]
        [Route("Update/{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] ProfileDto profile)
        {
            if (profile == null)
            {
                return BadRequest("Profile cannot be null.");
            }
            var existingProfile = await _unit.Profiles.GetFirstAsync().ConfigureAwait(true);
            if (existingProfile == null)
            {
                return NotFound($"Profile With ID --- {id} --- Not Exist Or Not Finish Yet.");
            }
            await _unit.Profiles.UpdateAsync(id, profile).ConfigureAwait(true);
            await _unit.CompleteAsync().ConfigureAwait(true);
            return Ok();
        }
        

    }
}
