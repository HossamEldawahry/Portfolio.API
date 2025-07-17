namespace Portfolio.API.Interfaces
{
    public interface IEditInDataBase<T> where T : class
    {
        Task UpdateAsync(int id,T dto) ;
    
    }
}
