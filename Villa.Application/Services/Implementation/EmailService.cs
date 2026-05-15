using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Threading;
using System.Threading.Tasks;
using Villla.Application.Services.Interface;
using Villla.Application.Settings;

namespace Villla.Infrastructure.CommonImplementation.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<EmailSettings> settings, ILogger<EmailService> logger)
        {
            _settings = settings.Value;
            _logger = logger;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var threadId = Thread.CurrentThread.ManagedThreadId;

            try
            {
                _logger.LogInformation(
                    "Email sending started | To: {Email}, Subject: {Subject}, ThreadId: {ThreadId}, Time: {Time}",
                    email, subject, threadId, DateTime.UtcNow
                );

                var emailMessage = new MimeMessage();

                emailMessage.From.Add(new MailboxAddress(
                    _settings.SenderName,
                    _settings.Email
                ));

                emailMessage.To.Add(MailboxAddress.Parse(email));
                emailMessage.Subject = subject;

                emailMessage.Body = new TextPart("html")
                {
                    Text = message
                };

                using var client = new MailKit.Net.Smtp.SmtpClient();

                await client.ConnectAsync(
                    _settings.Host,
                    _settings.Port,
                    SecureSocketOptions.StartTls
                );

                await client.AuthenticateAsync(
                    _settings.Email,
                    _settings.Password?.Replace(" ", "")
                );

                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);

                _logger.LogInformation(
                    "Email sent successfully | To: {Email}, Subject: {Subject}",
                    email, subject
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Email sending failed | To: {Email}, Subject: {Subject}, ThreadId: {ThreadId}",
                    email, subject, threadId
                );

                throw; // مهم: نسيبه يطلع للـ upper layer لو محتاج handle
            }
        }
    }
}