using CinemaBookingTicket_API.Data.Models;
using CinemaBookingTicket_API.Repository.IRepository;

namespace CinemaBookingTicket_API.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
