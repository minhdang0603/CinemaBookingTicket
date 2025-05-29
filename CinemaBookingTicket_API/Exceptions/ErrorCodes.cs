using System.Net;

namespace CinemaBookingTicket_API.Exceptions
{
    public static class ErrorCodes
    {
        public static Error UserNotFound(Guid userId) => new($"The user with id = {userId} was not found", HttpStatusCode.NotFound);
        public static Error UserAlreadyExists(string email) => new($"The user with email = {email} already exists", HttpStatusCode.Conflict);
        public static Error MovieNotFound(Guid movieId) => new($"The movie with id = {movieId} was not found", HttpStatusCode.NotFound);
        public static Error MovieAlreadyExists(string title) => new($"The movie with title = {title} already exists", HttpStatusCode.Conflict);
    }
}
