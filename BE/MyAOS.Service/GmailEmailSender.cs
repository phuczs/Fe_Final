using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace MyAOS.Service
{
    public class GmailEmailSender : IEmailSender
    {
        private readonly IConfiguration _config;
        private readonly ILogger<GmailEmailSender> _logger;

        public GmailEmailSender(
            IConfiguration config,
            ILogger<GmailEmailSender> logger)
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

            var mockStr = _config["GmailApi:Mock"];
            var isMock = bool.TryParse(mockStr, out var parsedMock) ? parsedMock : true; // Default to true if not configured

            if (isMock)
            {
                _logger.LogInformation("\n========== MOCK GMAIL API EMAIL ==========\nTO: {Recipient}\nSUBJECT: {Subject}\nBODY:\n{Body}\n==========================================", toEmail, subject, body);
                return;
            }

            var fromAddress = _config["GmailApi:FromAddress"];
            var fromName = _config["GmailApi:FromName"] ?? "MOS System";

            if (string.IsNullOrWhiteSpace(fromAddress))
            {
                throw new InvalidOperationException("GmailApi:FromAddress is missing.");
            }

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(fromName, fromAddress));
            message.To.Add(new MailboxAddress(string.Empty, toEmail));
            message.Subject = subject.Trim();

            var bodyBuilder = new BodyBuilder();
            var trimmedBody = body?.TrimStart() ?? string.Empty;
            if (trimmedBody.StartsWith("<") || trimmedBody.StartsWith("<!DOCTYPE"))
            {
                bodyBuilder.HtmlBody = body;
            }
            else
            {
                bodyBuilder.TextBody = body;
                bodyBuilder.HtmlBody = $"<div style=\"font-family: sans-serif; font-size: 14px; line-height: 1.5; color: #333333;\">{body?.Replace("\n", "<br/>")}</div>";
            }
            message.Body = bodyBuilder.ToMessageBody();

            string base64UrlMessage;
            using (var memory = new MemoryStream())
            {
                await message.WriteToAsync(memory, ct);
                var rawBytes = memory.ToArray();
                base64UrlMessage = Convert.ToBase64String(rawBytes)
                    .Replace('+', '-')
                    .Replace('/', '_')
                    .TrimEnd('=');
            }

            var accessToken = await GetAccessTokenAsync(ct);

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var payload = new GmailSendPayload { Raw = base64UrlMessage };
            var response = await client.PostAsJsonAsync("https://gmail.googleapis.com/gmail/v1/users/me/messages/send", payload, ct);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(ct);
                _logger.LogError("Failed to send email via Gmail API. Status: {StatusCode}, Error: {Error}", response.StatusCode, errorContent);
                throw new InvalidOperationException($"Failed to send email via Gmail API. Status: {response.StatusCode}, Details: {errorContent}");
            }

            _logger.LogInformation("Email sent successfully via Gmail API to {Recipient}", toEmail);
        }

        private async Task<string> GetAccessTokenAsync(CancellationToken ct)
        {
            var clientId = _config["GmailApi:ClientId"];
            var clientSecret = _config["GmailApi:ClientSecret"];
            var refreshToken = _config["GmailApi:RefreshToken"];

            if (string.IsNullOrWhiteSpace(clientId) ||
                string.IsNullOrWhiteSpace(clientSecret) ||
                string.IsNullOrWhiteSpace(refreshToken))
            {
                throw new InvalidOperationException("GmailApi configuration is incomplete (ClientId, ClientSecret, and RefreshToken are required).");
            }

            using var client = new HttpClient();
            var requestParams = new Dictionary<string, string>
            {
                { "client_id", clientId },
                { "client_secret", clientSecret },
                { "refresh_token", refreshToken },
                { "grant_type", "refresh_token" }
            };

            var response = await client.PostAsync("https://oauth2.googleapis.com/token", new FormUrlEncodedContent(requestParams), ct);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(ct);
                throw new InvalidOperationException($"Failed to refresh Gmail API OAuth token. Status: {response.StatusCode}, Error: {errorContent}");
            }

            var tokenResponse = await response.Content.ReadFromJsonAsync<GoogleTokenResponse>(cancellationToken: ct);
            if (tokenResponse == null || string.IsNullOrWhiteSpace(tokenResponse.AccessToken))
            {
                throw new InvalidOperationException("OAuth token response was empty or did not contain an access token.");
            }

            return tokenResponse.AccessToken;
        }

        private class GoogleTokenResponse
        {
            [JsonPropertyName("access_token")]
            public string AccessToken { get; set; } = string.Empty;

            [JsonPropertyName("expires_in")]
            public int ExpiresIn { get; set; }

            [JsonPropertyName("token_type")]
            public string TokenType { get; set; } = string.Empty;

            [JsonPropertyName("scope")]
            public string Scope { get; set; } = string.Empty;
        }

        private class GmailSendPayload
        {
            [JsonPropertyName("raw")]
            public string Raw { get; set; } = string.Empty;
        }
    }
}
