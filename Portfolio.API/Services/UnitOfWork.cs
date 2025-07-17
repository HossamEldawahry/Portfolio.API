
namespace Portfolio.API.Services
{
    public class UnitOfWork : IUnitOfWork
    {
        public IProjectRepository Projects { get; }
        public IProfileRepository Profiles { get; }
        public ISkillRepository Skills { get;}
        public IMessageRepository Messages { get; }
        private readonly AppDbContext _context;

        public UnitOfWork(AppDbContext context,
            IProjectRepository projectsRepo,
            ISkillRepository skills,
            IMessageRepository messages,
            IProfileRepository profiles)
        {
            _context = context;
            Projects = projectsRepo;
            Skills = skills;
            Messages = messages;
            Profiles = profiles;
        }
        Task<int> IUnitOfWork.CompleteAsync() => _context.SaveChangesAsync();

    }
}
