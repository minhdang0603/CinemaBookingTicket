using System.Net;

namespace API.Exceptions
{
    public class Error
    {
        public string Message { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public Error(string message, HttpStatusCode statusCode)
        {
            Message = message;
            StatusCode = statusCode;
        }
    }
}
