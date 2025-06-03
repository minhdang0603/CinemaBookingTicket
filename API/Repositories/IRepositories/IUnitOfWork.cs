using API.Repositories.IRepositories;

namespace API.Repositories.IRepositories
{
    public interface IUnitOfWork
    {
        IMovieRepository Movie { get; }
        Task SaveAsync();
    }
}
