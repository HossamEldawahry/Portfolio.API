namespace Portfolio.API.Interfaces
{
    public interface IGetOneRow <T> where T : class
    {
        Task<T?> GetByIdAsync(int id);
        Task<T?> GetFirstAsync();

    }
}
