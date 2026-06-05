using Microsoft.EntityFrameworkCore;
using MyAOS.Domain.Dto;
using MyAOS.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyAOS.Repository
{
    public interface IAuditLogRepository
    {
        Task AddAsync(AuditLogEntity entity, CancellationToken ct = default);

        Task<(IEnumerable<AuditLogEntity> Items, int TotalCount)> GetPagedAsync(Guid tenantId, GetAuditLogsQuery query, CancellationToken ct = default);

        Task<List<AuditLogEntity>> GetForExportAsync(Guid tenantId,GetAuditLogsQuery query,CancellationToken ct = default);

        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }

    public class AuditLogRepository : DatabaseRepositoryBase<AuditLogEntity>, IAuditLogRepository
    {
        public AuditLogRepository(AppDbContext dbContext) : base(dbContext)
        {
        }

        public async Task AddAsync(AuditLogEntity entity, CancellationToken ct = default)
        {
            await _dbSet.AddAsync(entity, ct);
        }

        public async Task<(IEnumerable<AuditLogEntity> Items, int TotalCount)> GetPagedAsync(Guid tenantId,GetAuditLogsQuery query,CancellationToken ct = default)
        {
            var page = query.Page <= 0 ? 1 : query.Page;
            var pageSize = query.PageSize <= 0 ? 10 : query.PageSize;

            if (pageSize > 100)
            {
                pageSize = 100;
            }

            var dbQuery = BuildQuery(tenantId, query);

            dbQuery = string.Equals(query.SortDirection, "asc", StringComparison.OrdinalIgnoreCase)
                ? dbQuery.OrderBy(x => x.OccurredAt)
                : dbQuery.OrderByDescending(x => x.OccurredAt);

            var totalCount = await dbQuery.CountAsync(ct);

            var items = await dbQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return (items, totalCount);
        }

        public async Task<List<AuditLogEntity>> GetForExportAsync(Guid tenantId,GetAuditLogsQuery query,CancellationToken ct = default)
        {
            return await BuildQuery(tenantId, query)
                .OrderByDescending(x => x.OccurredAt)
                .ToListAsync(ct);
        }

        public Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            return _context.SaveChangesAsync(ct);
        }

        private IQueryable<AuditLogEntity> BuildQuery(Guid tenantId, GetAuditLogsQuery query)
        {
            var dbQuery = _dbSet
                .AsNoTracking()
                .Where(x => x.TenantId == tenantId)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.ObjectType))
            {
                var objectType = query.ObjectType.Trim();
                dbQuery = dbQuery.Where(x => x.ObjectType.Contains(objectType));
            }

            if (!string.IsNullOrWhiteSpace(query.TargetName))
            {
                var targetName = query.TargetName.Trim();
                dbQuery = dbQuery.Where(x => x.TargetName.Contains(targetName));
            }

            if (query.TargetUserId.HasValue)
            {
                dbQuery = dbQuery.Where(x => x.TargetUserId == query.TargetUserId.Value);
            }

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var keyword = query.Search.Trim();

                dbQuery = dbQuery.Where(x =>
                    x.ObjectType.Contains(keyword) ||
                    x.TargetName.Contains(keyword) ||
                    (x.TargetUserId.HasValue && x.TargetUserId.Value.ToString().Contains(keyword)));
            }

            return dbQuery;
        }
    }

        // Notice: No Update or Delete methods exist here. 
        // This enforces the append-only rule at the C# level.
    }

