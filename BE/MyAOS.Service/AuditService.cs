using MyAOS.Domain.Entity;
using MyAOS.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyAOS.Service
{
    public class AuditService : IAuditService
    {
        private readonly IAuditLogRepository _auditLogRepo;

        public AuditService(IAuditLogRepository auditLogRepo)
        {
            _auditLogRepo = auditLogRepo;
        }

        public async Task LogAsync(Guid tenantId, string actorEmail, string objectType, string action, string targetName, Guid? targetUserId = null, string? changeDetail = null, CancellationToken ct = default)
        {
            var logEntry = new AuditLogEntity
            {
                TenantId = tenantId,
                ActorEmail = actorEmail,
                ObjectType = objectType,
                Action = action,
                TargetName = targetName,
                TargetUserId = targetUserId,
                ChangeDetail = changeDetail,
                OccurredAt = DateTime.UtcNow
            };

            await _auditLogRepo.AddAsync(logEntry, ct);
            await _auditLogRepo.SaveChangesAsync(ct);
        }
    }
}
