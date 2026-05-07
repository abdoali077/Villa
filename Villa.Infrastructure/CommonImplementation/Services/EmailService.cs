using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using MailKit.Net.Smtp;
//using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Villla.Application.Interfaces.Services;
using Villla.Infrastructure.Settings;

namespace Villla.Infrastructure.CommonImplementation.Services
{
    //public class EmailService : IEmailService
    //{
    //    private readonly EmailSettings _settings;

    //    public EmailService(IOptions<EmailSettings> settings)
    //    {
    //        _settings = settings.Value;
    //    }

    //    public async Task SendEmailAsync(string email, string subject, string message)
    //    {
    //        // 1. Create SMTP Client
    //        var smtpClient = new SmtpClient(_settings.Host, _settings.Port)
    //        {
    //            Credentials = new NetworkCredential(_settings.Email, _settings.Password),
    //            EnableSsl = true
    //        };

    //        // 2. Create Mail Message
    //        var mailMessage = new MailMessage
    //        {
    //            From = new MailAddress(_settings.Email, _settings.SenderName),
    //            Subject = subject,
    //            Body = message,
    //            IsBodyHtml = true
    //        };

    //        // 3. Add Receiver
    //        mailMessage.To.Add(email);

    //        // 4. Send Email
    //        await smtpClient.SendMailAsync(mailMessage);
    //    }
    //}
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;

        public EmailService(IOptions<EmailSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
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
                _settings.Password.Replace(" ", "")
            );

            await client.SendAsync(emailMessage);

            await client.DisconnectAsync(true);
        }
    }
}
