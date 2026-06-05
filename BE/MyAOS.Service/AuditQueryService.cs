using MyAOS.Domain.Dto;
using MyAOS.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyAOS.Service
{
    public class AuditQueryService : IAuditQueryService
    {
        private readonly IAuditLogRepository _auditLogRepo;

        public AuditQueryService(IAuditLogRepository auditLogRepo)
        {
            _auditLogRepo = auditLogRepo;
        }

        public async Task<PagedResultDto<AuditLogItemDto>> GetAuditLogsAsync(
            Guid currentTenantId,
            GetAuditLogsQuery query,
            CancellationToken ct = default)
        {
            var page = query.Page <= 0 ? 1 : query.Page;
            var pageSize = query.PageSize <= 0 ? 10 : query.PageSize;

            if (pageSize > 100)
            {
                pageSize = 100;
            }

            query.Page = page;
            query.PageSize = pageSize;

            var result = await _auditLogRepo.GetPagedAsync(currentTenantId, query, ct);

            return new PagedResultDto<AuditLogItemDto>
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = result.TotalCount,
                Items = result.Items.Select(x => new AuditLogItemDto
                {
                    Id = x.Id,
                    TenantId = x.TenantId,
                    ActorEmail = x.ActorEmail,
                    ObjectType = x.ObjectType,
                    Action = x.Action,
                    TargetName = x.TargetName,
                    TargetUserId = x.TargetUserId,
                    ChangeDetail = x.ChangeDetail,
                    OccurredAt = x.OccurredAt
                }).ToList()
            };
        }

        public async Task<byte[]> ExportAuditLogsCsvAsync(
            Guid currentTenantId,
            GetAuditLogsQuery query,
            CancellationToken ct = default)
        {
            var logs = await _auditLogRepo.GetForExportAsync(currentTenantId, query, ct);

            var sb = new StringBuilder();

            sb.AppendLine("Id,TenantId,ActorEmail,ObjectType,Action,TargetName,TargetUserId,ChangeDetail,OccurredAt");

            foreach (var log in logs)
            {
                sb.AppendLine(string.Join(",",
                    EscapeCsv(log.Id.ToString()),
                    EscapeCsv(log.TenantId.ToString()),
                    EscapeCsv(log.ActorEmail),
                    EscapeCsv(log.ObjectType),
                    EscapeCsv(log.Action),
                    EscapeCsv(log.TargetName),
                    EscapeCsv(log.TargetUserId?.ToString() ?? string.Empty),
                    EscapeCsv(log.ChangeDetail ?? string.Empty),
                    EscapeCsv(log.OccurredAt.ToString("O"))
                ));
            }

            return Encoding.UTF8.GetBytes(sb.ToString());
        }

        private static string EscapeCsv(string value)
        {
            if (value.Contains(',') || value.Contains('"') || value.Contains('\n') || value.Contains('\r'))
            {
                return $"\"{value.Replace("\"", "\"\"")}\"";
            }

            return value;
        }
    }
}
