namespace Portfolio.API.Services
{
    public interface IUnitOfWork
    {
        IProjectRepository Projects { get; }
        ISkillRepository Skills { get; }
        IMessageRepository Messages { get; }
        Task<int> CompleteAsync();
    }
}
