using Microsoft.AspNetCore.Identity;

namespace API.Data.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }

}
