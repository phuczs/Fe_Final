using System;
using System.Collections.Generic;
using System.Text;

namespace MyAOS.Domain.Entity
{
    public sealed class AuditLogEntity
    {
        public long Id { get; set; }
        public Guid TenantId { get; set; }
        public Guid? ActorId { get; set; }
        public string? ActorEmail { get; set; }

        // ObjectType: "User" | "Session" | "EmailWhitelist" | "UserProduct" | "Tenant"
        public string ObjectType { get; set; } = string.Empty;

        // Action: "SignIn" | "SignOut" | "Create" | "Update" | "Delete" | "Deactivate" | "BatchDelete" | "BatchDeactivate"
        public string Action { get; set; } = string.Empty;

        public string? TargetName { get; set; }
        public Guid? TargetUserId { get; set; }

        /// <summary>Optional JSON blob of field-level changes (Update events).</summary>
        public string? ChangeDetail { get; set; }

        public DateTime OccurredAt { get; set; }
    }
}
