using System.Net;
using API.Exceptions;

namespace API.Exceptions
{
    public static class ErrorCodes
    {
        // Generic entity not found error
        public static Error EntityNotFound(string entityName, object id) => new($"The {entityName} with id = {id} was not found", HttpStatusCode.NotFound);

        // User errors
        public static Error UserAlreadyExists(string email) => new($"The user with email = {email} already exists", HttpStatusCode.Conflict);
        public static Error UserCreationFailed() => new("User creation failed. Please try again.", HttpStatusCode.InternalServerError);

        // Movie errors
        public static Error MovieAlreadyExists(string title) => new($"The movie with title = {title} already exists", HttpStatusCode.Conflict);

        // Genre errors
        public static Error GenreAlreadyExists(string name) => new($"The genre with name = {name} already exists", HttpStatusCode.Conflict);

        // Authentication errors
        public static Error InvalidCredentials() => new("Invalid email or password.", HttpStatusCode.Unauthorized);
        public static Error UnauthorizedAccess() => new("You do not have permission to access this resource.", HttpStatusCode.Forbidden);

        // Payment errors - special case for payment of a booking
        public static Error PaymentNotFound(int bookingId) => new($"The payment of booking with id = {bookingId} was not found", HttpStatusCode.NotFound);

        // Theater errors
        public static Error TheaterAlreadyExists(string name) => new($"The theater with name = {name} already exists", HttpStatusCode.Conflict);

        // ShowTime errors
        public static Error InvalidShowTimeRange(int movieId, int screenId) => new($"The showtime range for movie with id = {movieId} and screen with id = {screenId} is invalid. Start time must be before end time.", HttpStatusCode.BadRequest);
        public static Error InvalidStartForShowTime(int screenId, TimeOnly previousEndTime, TimeOnly startTime) => new($"The showtime for screen with id = {screenId} must start at least 30 minutes after the previous showtime end at {previousEndTime}. Current start time is {startTime}.", HttpStatusCode.BadRequest);

        // Concession Category errors
        public static Error ConcessionCategoryAlreadyExists(string name) => new($"The concession category with name = {name} already exists", HttpStatusCode.Conflict);

        // Concession errors
        public static Error ConcessionAlreadyExists(string name) => new($"The concession with name = {name} already exists", HttpStatusCode.Conflict);

        // Seat errors
        public static Error SeatIsNotAvailable() => new($"Some seats are not available for booking. Please try another!", HttpStatusCode.BadRequest);

        // General errors
        public static Error InternalServerError(string message = "An unexpected error occurred. Please try again later.") => new(message, HttpStatusCode.InternalServerError);
    }
}
