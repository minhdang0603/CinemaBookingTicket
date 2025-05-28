using System.Net;

namespace CinemaBookingTicket_API.DTO
{
    public class APIResponse<T> where T : class
    {
        public HttpStatusCode StatusCode { get; set; }
        public bool IsSuccess { get; set; } = true;
        public List<string> ErrorMessages { get; set; }
        public T Result { get; set; }
    }
}
