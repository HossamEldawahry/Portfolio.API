namespace Portfolio.API.Interfaces
{
    public interface IGetAll <T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> GetAllAsync(int page, int pageSize);
        Task<int> CountAsync();
    }
}
