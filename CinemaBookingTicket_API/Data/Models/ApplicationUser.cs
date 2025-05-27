using Microsoft.AspNetCore.Identity;

namespace CinemaBookingTicket_API.Data.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }

}
