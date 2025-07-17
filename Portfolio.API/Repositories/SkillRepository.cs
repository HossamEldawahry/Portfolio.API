

namespace Portfolio.API.Repositories
{
    public class SkillRepository : ISkillRepository
    {
        private readonly AppDbContext _context;
        public SkillRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Skill data) => await _context.Skills.AddAsync(data).ConfigureAwait(false);
        public async Task<int> CountAsync() => await _context.Skills.CountAsync().ConfigureAwait(false);
        public void Delete(Skill data) => _context.Skills.Remove(data);
        public async Task<IEnumerable<Skill>> GetAllAsync() => await _context.Skills.ToListAsync().ConfigureAwait(false);
        public async Task<IEnumerable<Skill>> GetAllAsync(int page, int pageSize) => 
             await _context.Skills
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync()
                .ConfigureAwait(false);

        public async Task<Skill?> GetByIdAsync(int id) => 
            await _context.Skills.FindAsync(id).ConfigureAwait(false);
        public async Task<Skill?> GetFirstAsync() => 
            await _context.Skills.FirstOrDefaultAsync().ConfigureAwait(false);

        public async Task UpdateAsync(int id, Skill dto)
        {
            var skill = await _context.Skills.FindAsync(id).ConfigureAwait(false);
            if (skill != null)
            {
                skill.Name = dto.Name;
                skill.Level = dto.Level;
                _context.Skills.Update(skill);
            }
            
        }
    }
}
