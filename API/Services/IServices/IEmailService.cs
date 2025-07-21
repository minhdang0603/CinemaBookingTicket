namespace API.Services.IServices
{
    public interface IEmailService
    {
        Task SendEmailAsync(string receiverEmail, string subject, string htmlMessage);
        Task SendWelcomeEmailAsync(string receiverEmail, string userName);
        Task SendEmailConfirmationAsync(string receiverEmail, string userName, string confirmationToken);
        Task SendPasswordResetEmailAsync(string receiverEmail, string userName, string resetToken);
        Task SendBookingConfirmationAsync(string receiverEmail, string userName, string bookingCode, DateTime showTime, string movieTitle, List<string> seats, decimal totalAmount);
        Task SendOrderSuccessThankYouEmailAsync(string receiverEmail, string userName, string bookingCode, string movieTitle, string moviePoster, List<string> genres, decimal moviePrice, List<string> seats, DateTime showTime, string theaterName, string screenName, decimal totalAmount);
    }
}
