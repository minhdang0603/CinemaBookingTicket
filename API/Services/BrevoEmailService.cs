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

        public BrevoEmailService(IConfiguration configuration, ILogger<BrevoEmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
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

        public async Task SendEmailConfirmationAsync(string receiverEmail, string userName, string confirmationData)
        {
            string subject = "Confirm Your Email Address - CinemaBookingTicket";

            // Parse userId and token from confirmationData (format: "userId:token")
            var parts = confirmationData.Split(':', 2);
            if (parts.Length != 2)
            {
                throw new ArgumentException("Invalid confirmation data format");
            }

            var userId = parts[0];
            var token = parts[1];

            // Get base URL from configuration or use default
            string baseUrl = _configuration.GetValue<string>("Frontend:BaseUrl") ?? "http://localhost:5082";
            string confirmationUrl = $"{baseUrl}/Public/Auth/VerifyEmail?userId={userId}&token={Uri.EscapeDataString(token)}";

            string htmlMessage = GetEmailTemplate("EmailConfirmation")
                .Replace("{{UserName}}", userName)
                .Replace("{{ConfirmationUrl}}", confirmationUrl)
                .Replace("{{CurrentYear}}", DateTime.Now.Year.ToString());

            await SendEmailAsync(receiverEmail, subject, htmlMessage);
        }

        public async Task SendPasswordResetEmailAsync(string receiverEmail, string userName, string resetToken)
        {
            string subject = "Reset Your Password - CinemaBookingTicket";

            // Get base URL from configuration or use default
            string baseUrl = _configuration.GetValue<string>("Frontend:BaseUrl") ?? "http://localhost:5082";
            string resetUrl = $"{baseUrl}/Public/Auth/ResetPassword?email={Uri.EscapeDataString(receiverEmail)}&token={Uri.EscapeDataString(resetToken)}";

            string htmlMessage = GetEmailTemplate("PasswordReset")
                .Replace("{{UserName}}", userName)
                .Replace("{{ResetUrl}}", resetUrl)
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
                "Welcome" => "<div style='font-family: Arial; color: #333; max-width: 600px; margin: 0 auto; background-color: #ffffff; padding: 30px; border-radius: 10px; box-shadow: 0 0 20px rgba(0,0,0,0.1);'><div style='text-align: center; margin-bottom: 30px;'><h1 style='color: #dc3545; margin-bottom: 10px;'>🎬 Welcome to CinemaBookingTicket!</h1><div style='width: 50px; height: 3px; background-color: #dc3545; margin: 0 auto;'></div></div><p style='font-size: 18px; margin-bottom: 20px;'>Hello <strong>{{UserName}}</strong>,</p><p style='margin-bottom: 20px;'>🎉 <strong>Congratulations!</strong> Your email has been successfully verified and your account is now active!</p><p style='margin-bottom: 20px;'>Thank you for joining our community of movie lovers. We're excited to have you on board!</p><div style='background-color: #f8f9fa; padding: 20px; border-radius: 8px; margin: 25px 0; border-left: 4px solid #dc3545;'><h3 style='margin-top: 0; color: #dc3545;'>🚀 What you can do now:</h3><ul style='margin: 10px 0; padding-left: 20px;'><li>🎭 Browse our latest movie collection</li><li>🎫 Book tickets for your preferred showtimes</li><li>💺 Select your favorite seats</li><li>👤 Manage your bookings and profile</li><li>🔔 Get notified about new releases and special offers</li></ul></div><div style='text-align: center; margin: 30px 0;'><a href='http://localhost:5082' style='background-color: #dc3545; color: white; padding: 15px 30px; text-decoration: none; border-radius: 8px; display: inline-block; font-weight: bold; font-size: 16px; box-shadow: 0 4px 8px rgba(220, 53, 69, 0.3);'>🎬 Start Exploring Movies</a></div><p style='margin-bottom: 20px;'>Ready to discover your next favorite movie? Our cinema offers the latest blockbusters, indie films, and timeless classics!</p><p style='margin-bottom: 30px;'>Best regards,<br><strong>The CinemaBookingTicket Team</strong> 🍿</p><div style='margin-top: 30px; padding-top: 20px; border-top: 1px solid #eee; font-size: 12px; color: #666; text-align: center;'><p>© {{CurrentYear}} CinemaBookingTicket. All rights reserved.</p><p>This is an automated message. Please do not reply to this email.</p></div></div>",

                "EmailConfirmation" => "<div style='font-family: Arial; color: #333;'><h2>Confirm Your Email Address</h2><p>Hello {{UserName}},</p><p>Thank you for registering with CinemaBookingTicket! To complete your registration, please click the button below to verify your email address:</p><div style='text-align: center; margin: 30px 0;'><a href='{{ConfirmationUrl}}' style='background-color: #28a745; color: white; padding: 15px 30px; text-decoration: none; border-radius: 8px; display: inline-block; font-weight: bold; font-size: 16px; box-shadow: 0 4px 8px rgba(40, 167, 69, 0.3); transition: all 0.3s ease;'>✅ Confirm Email Address</a></div><p>If the button doesn't work, you can also copy and paste this link into your browser:</p><p style='word-break: break-all; color: #007bff; background-color: #f8f9fa; padding: 10px; border-radius: 5px; border-left: 4px solid #007bff;'>{{ConfirmationUrl}}</p><p><strong>⏰ Important:</strong> This link will expire in 24 hours for security reasons.</p><p>If you didn't create an account with us, please ignore this email.</p><p>Best regards,<br><strong>CinemaBookingTicket Team</strong> 🎬</p><div style='margin-top: 20px; font-size: 12px; color: #666;'>© {{CurrentYear}} CinemaBookingTicket. All rights reserved.</div></div>",

                "PasswordReset" => "<div style='font-family: Arial; color: #333; max-width: 600px; margin: 0 auto; background-color: #ffffff; padding: 30px; border-radius: 10px; box-shadow: 0 0 20px rgba(0,0,0,0.1);'><div style='text-align: center; margin-bottom: 30px;'><h1 style='color: #dc3545; margin-bottom: 10px;'>🔐 Reset Your Password</h1><div style='width: 50px; height: 3px; background-color: #dc3545; margin: 0 auto;'></div></div><p style='font-size: 18px; margin-bottom: 20px;'>Hello <strong>{{UserName}}</strong>,</p><p style='margin-bottom: 20px;'>We received a request to reset your password for your CinemaBookingTicket account.</p><p style='margin-bottom: 20px;'>If you made this request, click the button below to reset your password:</p><div style='text-align: center; margin: 30px 0;'><a href='{{ResetUrl}}' style='background-color: #ffc107; color: #212529; padding: 15px 30px; text-decoration: none; border-radius: 8px; display: inline-block; font-weight: bold; font-size: 16px; box-shadow: 0 4px 8px rgba(255, 193, 7, 0.3);'>🔑 Reset Password</a></div><div style='background-color: #fff3cd; padding: 20px; border-radius: 8px; margin: 25px 0; border-left: 4px solid #ffc107;'><p style='margin: 0; color: #856404;'><strong>⚠️ Important Security Information:</strong></p><ul style='margin: 10px 0; padding-left: 20px; color: #856404;'><li>This link will expire in <strong>10 minutes</strong></li><li>If you didn't request this reset, please ignore this email</li><li>Your password will remain unchanged unless you click the link above</li><li>Never share this reset link with anyone</li></ul></div><p style='margin-bottom: 20px;'>If the button doesn't work, copy and paste this link into your browser:</p><p style='word-break: break-all; color: #007bff; background-color: #f8f9fa; padding: 10px; border-radius: 5px; border-left: 4px solid #007bff;'>{{ResetUrl}}</p><p style='margin-bottom: 30px;'>Best regards,<br><strong>The CinemaBookingTicket Team</strong> 🎬</p><div style='margin-top: 30px; padding-top: 20px; border-top: 1px solid #eee; font-size: 12px; color: #666; text-align: center;'><p>© {{CurrentYear}} CinemaBookingTicket. All rights reserved.</p><p>This is an automated message. Please do not reply to this email.</p></div></div>",

                "BookingConfirmation" => "<div style='font-family: Arial; color: #333;'><h2>Booking Confirmation</h2><p>Hello {{UserName}},</p><p>Your booking has been confirmed!</p><p><strong>Booking Code:</strong> {{BookingCode}}</p><p><strong>Movie:</strong> {{MovieTitle}}</p><p><strong>Date & Time:</strong> {{ShowTime}}</p><p><strong>Seats:</strong> {{Seats}}</p><p><strong>Total Amount:</strong> {{TotalAmount}}</p><p>Thank you for choosing CinemaBookingTicket!</p><p>Best regards,<br>CinemaBookingTicket Team</p><div style='margin-top: 20px; font-size: 12px; color: #666;'>© {{CurrentYear}} CinemaBookingTicket. All rights reserved.</div></div>",

                _ => "<div style='font-family: Arial; color: #333;'><h2>CinemaBookingTicket Notification</h2><p>This is an automated message from CinemaBookingTicket.</p></div>"
            };
        }
    }
}
