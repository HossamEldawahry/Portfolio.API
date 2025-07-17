namespace Portfolio.API.Interfaces
{
    public interface IDeleteFromDataBase<T> where T : class
    {
        void Delete(T data);
    }
    
}
