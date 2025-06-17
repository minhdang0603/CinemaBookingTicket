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
        public static Error MovieIdNotFound(int movieId) => new($"The movie with id = {movieId} was not found", HttpStatusCode.NotFound);
        public static Error ScreenIdNotFound(int screenId) => new($"The screen with id = {screenId} was not", HttpStatusCode.Conflict);
        public static Error ShowTimeNotFound(int ShowTimId) => new($"The showtime with id = {ShowTimId} was not", HttpStatusCode.NotFound);
        public static Error InvalidShowTimeRange(int movieId, int screenId) => new($"The showtime range for movie with id = {movieId} and screen with id = {screenId} is invalid. Start time must be before end time.", HttpStatusCode.BadRequest);
        public static Error InvalidStartForShowTime(int screenId, TimeOnly endTime, TimeOnly startTime) => new($"The showtime for screen with id = {screenId} must start at least 30 minutes after the previous showtime ending at {endTime}. Current start time is {startTime}.", HttpStatusCode.BadRequest);
    }
}
