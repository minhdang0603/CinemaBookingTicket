using API.Repositories.IRepositories;
using API.Data.Models;
using API.Repositories;

namespace API.Repositories
{
    public class PaymentRepository : Repository<Payment>, IPaymentRepository
    {
        private readonly ApplicationDbContext _context;

        public PaymentRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
