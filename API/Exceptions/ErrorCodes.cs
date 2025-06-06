using System.Net;
using API.Exceptions;

namespace API.Exceptions
{
    public static class ErrorCodes
    {
        public static Error UserNotFound(Guid userId) => new($"The user with id = {userId} was not found", HttpStatusCode.NotFound);
        public static Error UserAlreadyExists(string email) => new($"The user with email = {email} already exists", HttpStatusCode.Conflict);
        public static Error MovieNotFound(int movieId) => new($"The movie with id = {movieId} was not found", HttpStatusCode.NotFound);
        public static Error MovieAlreadyExists(string title) => new($"The movie with title = {title} already exists", HttpStatusCode.Conflict);
        public static Error InvalidCredentials() => new("Invalid email or password.", HttpStatusCode.Unauthorized);
        public static Error UnauthorizedAccess() => new("You do not have permission to access this resource.", HttpStatusCode.Forbidden);
        public static Error UserCreationFailed() => new("User creation failed. Please try again.", HttpStatusCode.InternalServerError);
    }
}
