namespace Portfolio.API.Services
{
    public interface IUnitOfWork
    {
        IProjectRepository Projects { get; }
        IProfileRepository Profiles { get; }
        ISkillRepository Skills { get; }
        IMessageRepository Messages { get; }
        Task<int> CompleteAsync();
    }
}
