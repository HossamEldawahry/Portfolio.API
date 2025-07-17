namespace Portfolio.API.Interfaces
{
    public interface IAddToDataBase<T> where T : class
    {
        ///<summary>
        ///TO Add Data To Database Asynchronously, | 
        ///Use T As A Model To Add Data, | 
        ///Inhert This Interface In Your Repository, |
        ///</summary>
        Task AddAsync(T data);
    }
}
