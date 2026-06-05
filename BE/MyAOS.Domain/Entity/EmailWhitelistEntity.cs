using System;
using System.Collections.Generic;
using System.Text;

namespace MyAOS.Domain.Entity
{
    public sealed class EmailWhitelistEntity
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public string Email { get; set; } = string.Empty;
        public DateTime AddedAt { get; set; }
        public Guid? AddedBy { get; set; }

        // Navigation
        public TenantEntity Tenant { get; set; } = null!;
    }
}
