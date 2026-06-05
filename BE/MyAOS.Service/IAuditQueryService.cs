using MyAOS.Domain.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyAOS.Service
{
    public interface IAuditQueryService
    {
        Task<PagedResultDto<AuditLogItemDto>> GetAuditLogsAsync(Guid currentTenantId,GetAuditLogsQuery query, CancellationToken ct = default);

        Task<byte[]> ExportAuditLogsCsvAsync(Guid currentTenantId,GetAuditLogsQuery query,CancellationToken ct = default);
    }
}
