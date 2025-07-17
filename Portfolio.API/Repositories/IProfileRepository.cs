namespace Portfolio.API.Repositories
{
    public interface IProfileRepository : IAddToDataBase<ProfileDto>, IEditInDataBase<ProfileDto>, IGetOneRow<Profile>, IDeleteFromDataBase<Profile>
    {
    }
}
