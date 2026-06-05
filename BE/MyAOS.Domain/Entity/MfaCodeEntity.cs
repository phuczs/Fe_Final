using System;
using System.Collections.Generic;
using System.Text;

namespace MyAOS.Domain.Entity
{
    public sealed class MfaCodeEntity
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Code { get; set; } = string.Empty;   // padded to 6 chars
        public DateTime ExpiresAt { get; set; }
        public bool Used { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation
        public UserEntity User { get; set; } = null!;
    }
}
