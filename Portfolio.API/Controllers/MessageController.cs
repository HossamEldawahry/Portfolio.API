using Portfolio.API.Models;

namespace Portfolio.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IUnitOfWork _unit;
        public MessageController(IUnitOfWork unit)
        {
            _unit = unit;
        }
        [HttpGet("GetAll")]
        public async Task<ActionResult<IEnumerable<Message>>> GetAll()
        {

            var message = await _unit.Messages.GetAllAsync().ConfigureAwait(true);
            if (message == null)
            {
                return NotFound($"Message Are Not Exist");
            }
            return Ok(message);
        }
        [HttpGet()]
        [Route("GetById/{id}")]
        public async Task<ActionResult<Message>> GetById(int id)
        {
            var message = await _unit.Messages.GetByIdAsync(id).ConfigureAwait(true);
            if (message == null)
            {
                return NotFound($"Message With ID --- {id} --- Not Exist");
            }
            return Ok(message);
        }
        [HttpGet("count")]
        public async Task<ActionResult<int>> Count()
        {
            var count = await _unit.Messages.CountAsync().ConfigureAwait(true);
            return Ok(count);
        }
        [HttpPost("Create")]
        public async Task<ActionResult<Message>> Create(Message message)
        {
            if (message == null)
            {
                return BadRequest("Message cannot be null.");
            }
            await _unit.Messages.AddAsync(message).ConfigureAwait(true);
            await _unit.CompleteAsync().ConfigureAwait(true);
            return Ok();
        }
        [HttpPut()]
        [Route("Update/{id}")]
        public async Task<IActionResult> Update(int id, Message message)
        {
            if (message == null)
            {
                return BadRequest("Message cannot be null.");
            }
            await _unit.Messages.UpdateAsync(id, message).ConfigureAwait(true);
            await _unit.CompleteAsync().ConfigureAwait(true);
            return Ok();
        }
        [HttpDelete()]
        [Route("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var message = await _unit.Messages.GetByIdAsync(id).ConfigureAwait(true);
            if (message == null)
            {
                return NotFound($"Message With ID --- {id} --- Not Exist");
            }
            _unit.Messages.Delete(message);
            await _unit.CompleteAsync().ConfigureAwait(true);
            return NoContent();
        }
    }
}
