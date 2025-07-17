namespace Portfolio.API.Repositories
{
    public interface IMessageRepository : IAddToDataBase<Message>, IGetAll<Message>, IEditInDataBase<Message>, IGetOneRow<Message>, IDeleteFromDataBase<Message>
    {
    }
}
