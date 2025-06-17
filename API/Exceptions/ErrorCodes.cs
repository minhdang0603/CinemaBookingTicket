using System.Net;
using API.Exceptions;

namespace API.Exceptions
{
    public static class ErrorCodes
    {
        public static Error UserNotFound(string userId) => new($"The user with id = {userId} was not found", HttpStatusCode.NotFound);
        public static Error UserAlreadyExists(string email) => new($"The user with email = {email} already exists", HttpStatusCode.Conflict);
        public static Error MovieNotFound(int movieId) => new($"The movie with id = {movieId} was not found", HttpStatusCode.NotFound);
        public static Error MovieAlreadyExists(string title) => new($"The movie with title = {title} already exists", HttpStatusCode.Conflict);
        public static Error GenreAlreadyExists(string name) => new($"The genre with name = {name} already exists", HttpStatusCode.Conflict);
        public static Error GenreNotFound(int id) => new($"The genre with id = {id} was not found", HttpStatusCode.NotFound);
        public static Error InvalidCredentials() => new("Invalid email or password.", HttpStatusCode.Unauthorized);
        public static Error UnauthorizedAccess() => new("You do not have permission to access this resource.", HttpStatusCode.Forbidden);
        public static Error BookingNotFound(int bookingId) => new($"The booking with id = {bookingId} was not found", HttpStatusCode.NotFound);
        public static Error PaymentNotFound(int bookingId) => new($"The payment of booking with id = {bookingId} was not found", HttpStatusCode.NotFound);
        public static Error UserCreationFailed() => new("User creation failed. Please try again.", HttpStatusCode.InternalServerError);
        public static Error TheaterNotFound(int theaterId) => new($"The theater with id = {theaterId} was not found", HttpStatusCode.NotFound);
        public static Error ScreenNotFound(int screenId) => new($"The screen with id = {screenId} was not found", HttpStatusCode.NotFound);
        public static Error TheaterAlreadyExists(string name)
            => new($"The theater with name = {name} already exists", HttpStatusCode.Conflict);
        public static Error InternalServerError(string message = "An unexpected error occurred. Please try again later.")
            => new(message, HttpStatusCode.InternalServerError);
    }
}
