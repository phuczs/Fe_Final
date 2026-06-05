using System;
using System.Collections.Generic;
using System.Text;

namespace MyAOS.Domain.Entity
{
    public sealed class TenantEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool EmailWhitelistEnabled { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation
        public ICollection<UserEntity> Users { get; set; } = new List<UserEntity>();
        public ICollection<EmailWhitelistEntity> EmailWhitelist { get; set; } = new List<EmailWhitelistEntity>();
    }
}
