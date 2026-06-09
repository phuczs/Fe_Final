using System;

namespace MyAOS.Domain.Entity
{
    public class NotificationEntity
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
        public Guid? SentBy { get; set; }

        public TenantEntity Tenant { get; set; } = null!;
    }
}
