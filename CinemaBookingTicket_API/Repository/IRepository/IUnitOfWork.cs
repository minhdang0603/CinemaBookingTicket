namespace CinemaBookingTicket_API.Repository.IRepository
{
    public interface IUnitOfWork
    {
        Task SaveAsync();
    }
}
