using API.Repositories.IRepositories;

namespace CinemaBookingTicket_API.Repositories.IRepositories
{
    public interface IUnitOfWork
    {
        IMovieRepository Movie { get; }
        Task SaveAsync();
    }
}
