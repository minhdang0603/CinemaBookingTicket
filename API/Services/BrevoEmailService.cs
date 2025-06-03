using brevo_csharp.Api;
using brevo_csharp.Model;
using API.Services.IServices;

using Task = System.Threading.Tasks.Task;

namespace API.Services
{
    public class BrevoEmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<BrevoEmailService> _logger;

        public BrevoEmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string receiverEmail, string subject, string htmlMessage)
        {
            var apiInstance = new TransactionalEmailsApi();

            string senderName = _configuration.GetValue<string>("BrevoApi:SenderName") ?? "CinemaBookingTicket";
            string senderEmail = _configuration.GetValue<string>("BrevoApi:SenderEmail") ?? "ruabin1163@gmail.com";

            SendSmtpEmailSender sender = new SendSmtpEmailSender(senderName, senderEmail);
            SendSmtpEmailTo receiver = new SendSmtpEmailTo(receiverEmail);
            List<SendSmtpEmailTo> to = new List<SendSmtpEmailTo> { receiver };

            try
            {
                var sendSmtpEmail = new SendSmtpEmail(
                    sender: sender,
                    to: to,
                    htmlContent: htmlMessage,
                    subject: subject
                );

                await apiInstance.SendTransacEmailAsync(sendSmtpEmail);
                _logger.LogInformation($"Email sent successfully to {receiverEmail}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send email to {receiverEmail}");
                throw;
            }
        }

        public async Task SendWelcomeEmailAsync(string receiverEmail, string userName)
        {
            string subject = "Welcome to CinemaBookingTicket";
            string htmlMessage = GetEmailTemplate("Welcome")
                .Replace("{{UserName}}", userName)
                .Replace("{{CurrentYear}}", DateTime.Now.Year.ToString());

            await SendEmailAsync(receiverEmail, subject, htmlMessage);
        }

        public async Task SendBookingConfirmationAsync(string receiverEmail, string userName, string bookingCode, DateTime showTime, string movieTitle, List<string> seats, decimal totalAmount)
        {
            string subject = $"Booking Confirmation #{bookingCode}";

            string seatsHtml = string.Join(", ", seats);

            string htmlMessage = GetEmailTemplate("BookingConfirmation")
                .Replace("{{UserName}}", userName)
                .Replace("{{BookingCode}}", bookingCode)
                .Replace("{{MovieTitle}}", movieTitle)
                .Replace("{{ShowTime}}", showTime.ToString("dd/MM/yyyy HH:mm"))
                .Replace("{{Seats}}", seatsHtml)
                .Replace("{{TotalAmount}}", totalAmount.ToString("N0") + " VND")
                .Replace("{{CurrentYear}}", DateTime.Now.Year.ToString());

            await SendEmailAsync(receiverEmail, subject, htmlMessage);
        }

        private string GetEmailTemplate(string templateName)
        {
            string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplates", $"{templateName}.html");

            if (File.Exists(templatePath))
            {
                return File.ReadAllText(templatePath);
            }

            _logger.LogWarning($"Email template {templateName}.html not found. Using fallback template.");
            return GetFallbackTemplate(templateName);
        }

        private string GetFallbackTemplate(string templateType)
        {
            return templateType switch
            {
                "Welcome" => "<div style='font-family: Arial; color: #333;'><h2>Welcome to CinemaBookingTicket!</h2><p>Hello {{UserName}},</p><p>Thank you for registering with us. We're excited to have you on board!</p><p>Start exploring movies and book your favorite seats today.</p><p>Best regards,<br>CinemaBookingTicket Team</p><div style='margin-top: 20px; font-size: 12px; color: #666;'>© {{CurrentYear}} CinemaBookingTicket. All rights reserved.</div></div>",

                "BookingConfirmation" => "<div style='font-family: Arial; color: #333;'><h2>Booking Confirmation</h2><p>Hello {{UserName}},</p><p>Your booking has been confirmed!</p><p><strong>Booking Code:</strong> {{BookingCode}}</p><p><strong>Movie:</strong> {{MovieTitle}}</p><p><strong>Date & Time:</strong> {{ShowTime}}</p><p><strong>Seats:</strong> {{Seats}}</p><p><strong>Total Amount:</strong> {{TotalAmount}}</p><p>Thank you for choosing CinemaBookingTicket!</p><p>Best regards,<br>CinemaBookingTicket Team</p><div style='margin-top: 20px; font-size: 12px; color: #666;'>© {{CurrentYear}} CinemaBookingTicket. All rights reserved.</div></div>",

                _ => "<div style='font-family: Arial; color: #333;'><h2>CinemaBookingTicket Notification</h2><p>This is an automated message from CinemaBookingTicket.</p></div>"
            };
        }
    }
}
