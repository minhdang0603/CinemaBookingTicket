using System.Net;

namespace CinemaBookingTicket_API.Exceptions
{
    public class BaseException : Exception
    {
        public HttpStatusCode StatusCode { get; set; }

        public BaseException(string message, HttpStatusCode statusCode = HttpStatusCode.InternalServerError) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
