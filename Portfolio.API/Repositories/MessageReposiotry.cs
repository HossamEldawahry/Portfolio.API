
namespace Portfolio.API.Repositories
{
    public class MessageReposiotry : IMessageRepository
    {
        private readonly AppDbContext _context;
        public MessageReposiotry(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Message data) => await _context.Messages.AddAsync(data).ConfigureAwait(false);
       
        public async Task<int> CountAsync() => await _context.Messages.CountAsync().ConfigureAwait(false);
       

        public void Delete(Message data) => _context.Messages.Remove(data);
        public async Task<IEnumerable<Message>> GetAllAsync() => await _context.Messages.OrderByDescending(m => m.CreatedAt).ToListAsync().ConfigureAwait(false);
        public async Task<IEnumerable<Message>> GetAllAsync(int page, int pageSize) => 
            await _context.Messages
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync()
                .ConfigureAwait(false);
        public async Task<Message?> GetByIdAsync(int id) => await _context.Messages.FindAsync(id).ConfigureAwait(false);
        public async Task<Message?> GetFirstAsync() => await _context.Messages.OrderByDescending(m => m.CreatedAt).FirstOrDefaultAsync().ConfigureAwait(false);
        public async Task UpdateAsync(int id, Message dto)
        {
            var message = await _context.Messages.FindAsync(id).ConfigureAwait(false);
            if (message != null)
            {
                message.Name = dto.Name;
                message.Email = dto.Email;
                message.Subject = dto.Subject;
                message.Content = dto.Content;
                _context.Messages.Update(message);
            }
        }
    }
}
