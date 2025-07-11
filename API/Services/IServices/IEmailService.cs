﻿namespace API.Services.IServices
{
    public interface IEmailService
    {
        Task SendEmailAsync(string receiverEmail, string subject, string htmlMessage);
        Task SendWelcomeEmailAsync(string receiverEmail, string userName);
        Task SendBookingConfirmationAsync(string receiverEmail, string userName, string bookingCode, DateTime showTime, string movieTitle, List<string> seats, decimal totalAmount);
    }
}
