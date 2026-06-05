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

        public WhitelistService(
            IEmailWhitelistRepository emailWhitelistRepo,
            ITenantRepository tenantRepo,
            IAuditService auditService,
            IEmailSender emailSender)
        {
            _emailWhitelistRepo = emailWhitelistRepo;
            _tenantRepo = tenantRepo;
            _auditService = auditService;
            _emailSender = emailSender;
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

            var whitelistEmails = await _emailWhitelistRepo.GetByTenantIdAsync(currentTenantId, ct);

            if (whitelistEmails == null || !whitelistEmails.Any())
            {
                return new SendWhitelistEmailResponse { SentCount = 0 };
            }

            var response = new SendWhitelistEmailResponse();

            foreach (var entry in whitelistEmails)
            {
                try
                {
                    await _emailSender.SendAsync(entry.Email, request.Subject, request.Body, ct);
                    response.SentTo.Add(entry.Email);
                }
                catch (Exception ex)
                {
                    response.FailedTo.Add(entry.Email);
                    response.FailedDetails[entry.Email] = ex.Message;
                }
            }

            response.SentCount = response.SentTo.Count;

            return response;
        }
    }
}
