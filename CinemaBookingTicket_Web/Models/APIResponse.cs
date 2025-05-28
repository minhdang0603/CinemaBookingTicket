using System.Net;

namespace CinemaBookingTicket_Web.Models
{
    public class APIResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public bool IsSuccess { get; set; } = true;
        public string ErrorMessages { get; set; }
        public object Result { get; set; }
    }
}
