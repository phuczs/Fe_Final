using MyAOS.Domain.Dto;
using MyAOS.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using MyAOS.Domain.Entity;
using System.Net.Mail;

namespace MyAOS.Service
{
    public class WhitelistService : IWhitelistService
    {
        private readonly IEmailWhitelistRepository _emailWhitelistRepo;
        private readonly ITenantRepository _tenantRepo;
        private readonly IAuditService _auditService;
        private readonly IEmailSender _emailSender;
        private readonly INotificationRepository _notificationRepo;
        private readonly IUserRepository _userRepo;

        public WhitelistService(
            IEmailWhitelistRepository emailWhitelistRepo,
            ITenantRepository tenantRepo,
            IAuditService auditService,
            IEmailSender emailSender,
            INotificationRepository notificationRepo,
            IUserRepository userRepo)
        {
            _emailWhitelistRepo = emailWhitelistRepo;
            _tenantRepo = tenantRepo;
            _auditService = auditService;
            _emailSender = emailSender;
            _notificationRepo = notificationRepo;
            _userRepo = userRepo;
        }

        public async Task<WhitelistSettingsDto> GetWhitelistSettingsAsync(
            Guid currentTenantId,
            string currentUserRole,
            CancellationToken ct = default)
        {
            if (!IsAdminRole(currentUserRole))
            {
                throw new UnauthorizedAccessException("Admin permission is required.");
            }

            var tenant = await _tenantRepo.GetByIdAsync(currentTenantId, ct);

            if (tenant == null)
            {
                throw new KeyNotFoundException("Tenant not found.");
            }

            var whitelistEmails = await _emailWhitelistRepo.GetByTenantIdAsync(
                currentTenantId,
                ct);

            return new WhitelistSettingsDto
            {
                TenantId = tenant.Id,
                EmailWhitelistEnabled = tenant.EmailWhitelistEnabled,
                Emails = whitelistEmails.Select(x => new WhitelistEmailDto
                {
                    Id = x.Id,
                    Email = x.Email,
                    CreatedAt = x.AddedAt
                }).ToList()
            };
        }
        public async Task<ToggleWhitelistResponseDto> ToggleWhitelistAsync(
    Guid currentTenantId,
    string currentUserRole,
    string actorEmail,
    Guid? actorUserId,
    ToggleWhitelistRequest request,
    CancellationToken ct = default)
        {
            if (!IsAdminRole(currentUserRole))
            {
                throw new UnauthorizedAccessException("Admin permission is required.");
            }

            var tenant = await _tenantRepo.GetByIdAsync(currentTenantId, ct);

            if (tenant == null)
            {
                throw new KeyNotFoundException("Tenant not found.");
            }

            var oldValue = tenant.EmailWhitelistEnabled;

            tenant.EmailWhitelistEnabled = request.Enabled;
            tenant.UpdatedAt = DateTime.UtcNow;

            await _tenantRepo.SaveChangesAsync(ct);

            await _auditService.LogAsync(
     tenant.Id,
     actorEmail,
     "Tenant",
     "Update",
     tenant.Name,
     actorUserId,
     "{" +
     $"\"Result\":\"Success\"," +
     $"\"OldEmailWhitelistEnabled\":{oldValue.ToString().ToLower()}," +
     $"\"NewEmailWhitelistEnabled\":{tenant.EmailWhitelistEnabled.ToString().ToLower()}," +
     $"\"UpdatedBy\":\"{actorEmail}\"" +
     "}",
     ct);

            return new ToggleWhitelistResponseDto
            {
                TenantId = tenant.Id,
                EmailWhitelistEnabled = tenant.EmailWhitelistEnabled
            };
        }
        public async Task<AddWhitelistEmailsResponseDto> AddEmailsAsync(
    Guid currentTenantId,
    string currentUserRole,
    string actorEmail,
    Guid? actorUserId,
    AddWhitelistEmailsRequest request,
    CancellationToken ct = default)
        {
            if (!IsAdminRole(currentUserRole))
            {
                throw new UnauthorizedAccessException("Admin permission is required.");
            }

            if (request.Emails == null || request.Emails.Count == 0)
            {
                throw new ArgumentException("Emails is required.");
            }

            var response = new AddWhitelistEmailsResponseDto
            {
                TenantId = currentTenantId
            };

            var emails = request.Emails
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim().ToLowerInvariant())
                .Distinct()
                .ToList();

            if (emails.Count == 0)
            {
                throw new ArgumentException("Emails is invalid.");
            }

            foreach (var email in emails)
            {
                if (!IsValidEmail(email))
                {
                    response.SkippedEmails.Add(email);
                    continue;
                }

                var exists = await _emailWhitelistRepo.IsEmailWhitelistedAsync(
                    currentTenantId,
                    email,
                    ct);

                if (exists)
                {
                    response.SkippedEmails.Add(email);
                    continue;
                }

                var entity = new EmailWhitelistEntity
                {
                    Id = Guid.NewGuid(),
                    TenantId = currentTenantId,
                    Email = email,
                    AddedAt = DateTime.UtcNow,
                    AddedBy = actorUserId
                };

                await _emailWhitelistRepo.AddAsync(entity, ct);

                response.AddedEmails.Add(email);
            }

            if (response.AddedEmails.Count > 0)
            {
                await _emailWhitelistRepo.SaveChangesAsync(ct);

                foreach (var addedEmail in response.AddedEmails)
                {
                    await _auditService.LogAsync(
                        currentTenantId,
                        actorEmail,
                        "EmailWhitelist",
                        "Create",
                        addedEmail,
                        actorUserId,
                        "{" +
                        $"\"Result\":\"Success\"," +
                        $"\"Email\":\"{addedEmail}\"," +
                        $"\"AddedBy\":\"{actorEmail}\"" +
                        "}",
                        ct);
                }
            }

            return response;
        }
        public async Task DeleteEmailAsync(
    Guid currentTenantId,
    string currentUserRole,
    string actorEmail,
    Guid? actorUserId,
    Guid whitelistEmailId,
    CancellationToken ct = default)
        {
            if (!IsAdminRole(currentUserRole))
            {
                throw new UnauthorizedAccessException("Admin permission is required.");
            }

            if (whitelistEmailId == Guid.Empty)
            {
                throw new ArgumentException("Whitelist email id is invalid.");
            }

            var whitelistEmail = await _emailWhitelistRepo.GetByIdAsync(
                currentTenantId,
                whitelistEmailId,
                ct);

            if (whitelistEmail == null)
            {
                throw new KeyNotFoundException("Whitelist email not found.");
            }

            await _emailWhitelistRepo.RemoveByIdAsync(
                currentTenantId,
                whitelistEmailId,
                ct);

            await _auditService.LogAsync(
                currentTenantId,
                actorEmail,
                "EmailWhitelist",
                "Delete",
                whitelistEmail.Email,
                actorUserId,
                "{" +
                $"\"Result\":\"Success\"," +
                $"\"Email\":\"{whitelistEmail.Email}\"," +
                $"\"DeletedBy\":\"{actorEmail}\"" +
                "}",
                ct);
        }

        private static bool IsValidEmail(string email)
        {
            try
            {
                var address = new MailAddress(email);
                return string.Equals(address.Address, email, StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        private static bool IsAdminRole(string role)
        {
            return
                string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(role, "Administrator", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(role, "TenantAdmin", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(role, "SystemAdmin", StringComparison.OrdinalIgnoreCase) ||
                role == "1" ||
                role == "2";
        }

        public async Task<SendWhitelistEmailResponse> SendEmailToWhitelistAsync(
            Guid currentTenantId,
            string currentUserRole,
            SendWhitelistEmailRequest request,
            CancellationToken ct = default)
        {
            if (!IsAdminRole(currentUserRole))
            {
                throw new UnauthorizedAccessException("Admin permission is required.");
            }

            if (string.IsNullOrWhiteSpace(request.Subject))
            {
                throw new ArgumentException("Subject is required.");
            }

            if (string.IsNullOrWhiteSpace(request.Body))
            {
                throw new ArgumentException("Body is required.");
            }

            var tenant = await _tenantRepo.GetByIdAsync(currentTenantId, ct);
            if (tenant == null)
            {
                throw new KeyNotFoundException("Tenant not found.");
            }

            List<string> recipientEmails;

            if (tenant.EmailWhitelistEnabled)
            {
                var whitelist = await _emailWhitelistRepo.GetByTenantIdAsync(currentTenantId, ct);
                recipientEmails = whitelist.Select(x => x.Email).ToList();
            }
            else
            {
                recipientEmails = await _userRepo.GetAllEmailsAsync(currentTenantId, ct);
            }

            if (recipientEmails == null || !recipientEmails.Any())
            {
                return new SendWhitelistEmailResponse { SentCount = 0 };
            }

            var footerNotice = tenant.EmailWhitelistEnabled
                ? "This message was sent to whitelisted members of the MOS Platform."
                : "This message was sent to all registered users of the MOS Platform.";

            var htmlBody = $@"<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"">
    <title>{request.Subject}</title>
</head>
<body style=""margin: 0; padding: 0; background-color: #f6f8fa; font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Helvetica, Arial, sans-serif;"">
    <table role=""presentation"" width=""100%"" cellspacing=""0"" cellpadding=""0"" style=""background-color: #f6f8fa; padding: 40px 0;"">
        <tr>
            <td align=""center"">
                <table role=""presentation"" width=""600"" cellspacing=""0"" cellpadding=""0"" style=""background-color: #ffffff; border-radius: 8px; border: 1px solid #d0d7de; box-shadow: 0 3px 6px rgba(140, 149, 159, 0.15); overflow: hidden;"">
                    <!-- Blue Accent Header Bar -->
                    <tr>
                        <td height=""4"" style=""background-color: #0969da; line-height: 4px; font-size: 0px;"">&nbsp;</td>
                    </tr>
                    <!-- Content Area -->
                    <tr>
                        <td style=""padding: 32px 32px 24px 32px;"">
                            <!-- Header Title -->
                            <div style=""font-size: 12px; font-weight: 600; color: #57606a; letter-spacing: 0.8px; text-transform: uppercase; margin-bottom: 16px;"">
                                System Notification
                            </div>
                            
                            <!-- Subject -->
                            <h2 style=""margin: 0 0 16px 0; font-size: 20px; font-weight: 600; color: #24292f; line-height: 1.4; border-bottom: 1px solid #d8dee4; padding-bottom: 12px;"">
                                {request.Subject}
                            </h2>
                            
                            <!-- Body text -->
                            <div style=""font-size: 15px; line-height: 1.6; color: #24292f; white-space: pre-wrap; word-break: break-word;"">
                                {request.Body}
                            </div>
                        </td>
                    </tr>
                    <!-- Footer Area -->
                    <tr>
                        <td style=""background-color: #f6f8fa; padding: 24px 32px; border-top: 1px solid #d8dee4; text-align: left;"">
                            <p style=""margin: 0 0 6px 0; font-size: 12px; line-height: 1.4; color: #57606a;"">
                                {footerNotice}
                            </p>
                            <p style=""margin: 0; font-size: 11px; color: #8c959f;"">
                                &copy; {DateTime.UtcNow.Year} MOS System. All rights reserved.
                            </p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";

            var response = new SendWhitelistEmailResponse();

            foreach (var email in recipientEmails)
            {
                try
                {
                    await _emailSender.SendAsync(email, request.Subject, htmlBody, ct);
                    response.SentTo.Add(email);
                }
                catch (Exception ex)
                {
                    response.FailedTo.Add(email);
                    response.FailedDetails[email] = ex.Message;
                }
            }

            if (response.SentTo.Count > 0)
            {
                var subject = request.Subject;
                if (tenant.EmailWhitelistEnabled)
                {
                    subject = "Restricted:" + subject;
                }

                var notification = new NotificationEntity
                {
                    Id = Guid.NewGuid(),
                    TenantId = currentTenantId,
                    Subject = subject,
                    Body = request.Body,
                    SentAt = DateTime.UtcNow
                };

                await _notificationRepo.AddAsync(notification, ct);
                await _notificationRepo.SaveChangesAsync(ct);
            }

            response.SentCount = response.SentTo.Count;

            return response;
        }
    }
}
