using System;
using System.Collections.Generic;
using System.Text;

namespace MyAOS.Domain.Entity
{
    public sealed class RefreshTokenEntity
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }

        public string TokenHash { get; set; } = string.Empty;

        public DateTime ExpiresAt { get; set; }
        public bool IsRevoked { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? RevokedAt { get; set; }

        public string? ReplacedByTokenHash { get; set; }

        public UserEntity User { get; set; } = null!;
    }
}