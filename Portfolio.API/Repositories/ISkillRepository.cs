namespace Portfolio.API.Repositories
{
    public interface ISkillRepository : IAddToDataBase<Skill>, IGetAll<Skill>, IEditInDataBase<Skill>, IGetOneRow<Skill>,IDeleteFromDataBase<Skill>
    {
    }
}
