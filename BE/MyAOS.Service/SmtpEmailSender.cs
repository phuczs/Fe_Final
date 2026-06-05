using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace MyAOS.Service
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly IConfiguration _config;
        private readonly ILogger<SmtpEmailSender> _logger;

        public SmtpEmailSender(
            IConfiguration config,
            ILogger<SmtpEmailSender> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task SendAsync(
            string toEmail,
            string subject,
            string body,
            CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(toEmail))
            {
                throw new ArgumentException("Recipient email is required.", nameof(toEmail));
            }

            if (string.IsNullOrWhiteSpace(subject))
            {
                throw new ArgumentException("Email subject is required.", nameof(subject));
            }

            var host = _config["Smtp:Host"];
            var fromAddress = _config["Smtp:FromAddress"];
            var fromName = _config["Smtp:FromName"] ?? "MOS System";
            var username = _config["Smtp:Username"];
            var password = _config["Smtp:Password"];

            if (string.IsNullOrWhiteSpace(host))
            {
                throw new InvalidOperationException("Smtp:Host is missing.");
            }

            if (string.IsNullOrWhiteSpace(fromAddress))
            {
                throw new InvalidOperationException("Smtp:FromAddress is missing.");
            }

            if (string.IsNullOrWhiteSpace(username))
            {
                throw new InvalidOperationException("Smtp:Username is missing.");
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new InvalidOperationException("Smtp:Password is missing.");
            }

            var port = int.TryParse(_config["Smtp:Port"], out var parsedPort)
                ? parsedPort
                : 587;

            var useStartTls = bool.TryParse(_config["Smtp:UseStartTls"], out var parsedUseStartTls)
                ? parsedUseStartTls
                : true;

            var secureSocketOption = useStartTls
                ? SecureSocketOptions.StartTls
                : SecureSocketOptions.SslOnConnect;

            var message = new MimeMessage();

            message.From.Add(new MailboxAddress(fromName, fromAddress));
            message.To.Add(new MailboxAddress(string.Empty, toEmail));
            message.Subject = subject.Trim();

            message.Body = new TextPart("plain")
            {
                Text = body ?? string.Empty
            };

            using var client = new SmtpClient();

            try
            {
                await client.ConnectAsync(host, port, secureSocketOption, ct);

                await client.AuthenticateAsync(username, password, ct);

                await client.SendAsync(message, ct);

                await client.DisconnectAsync(true, ct);

                _logger.LogInformation("Email sent successfully to {Recipient}", toEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Recipient}", toEmail);
                throw;
            }
        }
    }
}
