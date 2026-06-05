using System;
using System.Collections.Generic;
using System.Text;

namespace MyAOS.Domain.Dto
{
    public class AuditLogItemDto
    {
        public long Id { get; set; }

        public Guid TenantId { get; set; }

        public string ActorEmail { get; set; } = string.Empty;

        public string ObjectType { get; set; } = string.Empty;

        public string Action { get; set; } = string.Empty;

        public string TargetName { get; set; } = string.Empty;

        public Guid? TargetUserId { get; set; }

        public string? ChangeDetail { get; set; }

        public DateTime OccurredAt { get; set; }
    }
}
