using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Configuration;

namespace UmbracoCMS.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task<bool> SendEmailAsync(string to, string subject, string message)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(_config["SMTP:User"]));
                email.To.Add(MailboxAddress.Parse(to));
                email.Subject = subject;
                email.Body = new TextPart("html") { Text = message };

                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(
                    _config["SMTP:Host"],
                    int.Parse(_config["SMTP:Port"]),
                    SecureSocketOptions.StartTls);

                await smtp.AuthenticateAsync(_config["SMTP:User"], _config["SMTP:Pass"]);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
