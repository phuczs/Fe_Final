using System;
using System.Collections.Generic;
using System.Text;

namespace MyAOS.Domain.Entity
{
    public sealed class RegistrationVerificationCodeEntity
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public string Email { get; set; } = string.Empty;

        public string? PhoneNumber { get; set; }
        public string Code { get; set; } = string.Empty;
        public int AttemptCount { get; set; }
        public bool Verified { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
