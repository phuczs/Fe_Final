using System;
using System.Collections.Generic;
using System.Text;

namespace MyAOS.Service
{
    public interface IAuditService
    {
        Task LogAsync(Guid tenantId, string actorEmail, string objectType, string action, string targetName, Guid? targetUserId = null, string? changeDetail = null, CancellationToken ct = default);
    }
}
