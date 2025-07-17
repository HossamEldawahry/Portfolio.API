namespace Portfolio.API.Repositories
{
    public interface IProjectRepository : IAddToDataBase<ProjectsDto>, IGetAll<Project>, IGetOneRow<Project>, IEditInDataBase<ProjectsDto>,IDeleteFromDataBase<Project>
    {
    }
}
