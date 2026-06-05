using MyAOS.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace MyAOS.Repository
{
    public interface IEmailWhitelistRepository
    {
        Task<bool> IsEmailWhitelistedAsync(Guid tenantId, string email, CancellationToken ct = default);
        Task<(IEnumerable<EmailWhitelistEntity> Items, int TotalCount)> GetPagedAsync(Guid tenantId, int page, int pageSize, CancellationToken ct = default);
        Task AddAsync(EmailWhitelistEntity entity, CancellationToken ct = default);
        Task RemoveAsync(Guid tenantId, string email, CancellationToken ct = default);
        Task<int> SaveChangesAsync(CancellationToken ct = default);
        Task<EmailWhitelistEntity?> GetByIdAsync(Guid tenantId,Guid id,CancellationToken ct = default);

        Task RemoveByIdAsync(Guid tenantId, Guid id,CancellationToken ct = default);
        Task<List<EmailWhitelistEntity>> GetByTenantIdAsync(Guid tenantId, CancellationToken ct = default);
    }

    public class EmailWhitelistRepository : DatabaseRepositoryBase<EmailWhitelistEntity>, IEmailWhitelistRepository
    {
        public EmailWhitelistRepository(AppDbContext context) : base(context) { }

        public async Task<bool> IsEmailWhitelistedAsync(
            Guid tenantId,
            string email,
            CancellationToken ct = default)
        {
            return await _dbSet.AnyAsync(x =>
                x.TenantId == tenantId &&
                x.Email == email,
                ct);
        }

        public async Task<List<EmailWhitelistEntity>> GetByTenantIdAsync(
           Guid tenantId,
           CancellationToken ct = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(x => x.TenantId == tenantId)
                .OrderBy(x => x.Email)
                .ToListAsync(ct);
        }

        public async Task RemoveAsync(Guid tenantId, string email, CancellationToken ct = default)
        {
            // SECURE: Bypassing memory overhead by issuing a direct delete instruction based on Tenant + Email
            await _dbSet
                .Where(e => e.TenantId == tenantId && e.Email == email)
                .ExecuteDeleteAsync(ct);
        }

        public async Task<(IEnumerable<EmailWhitelistEntity> Items, int TotalCount)> GetPagedAsync(Guid tenantId, int page, int pageSize, CancellationToken ct = default)
        {
            var query = _dbSet
                .AsNoTracking()
                .Where(x => x.TenantId == tenantId);

            var total = await query.CountAsync(ct);

            var items = await query
                .OrderBy(x => x.Email)
                .Skip((Math.Max(page, 1) - 1) * Math.Max(pageSize, 0))
                .Take(Math.Max(pageSize, 0))
                .ToListAsync(ct);

            return (items, total);
        }
        public async Task<EmailWhitelistEntity?> GetByIdAsync(Guid tenantId,Guid id,CancellationToken ct = default)
        {
            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.TenantId == tenantId && x.Id == id, ct);
        }

        public async Task RemoveByIdAsync(Guid tenantId,Guid id,CancellationToken ct = default)
        {
            await _dbSet
                .Where(x => x.TenantId == tenantId && x.Id == id)
                .ExecuteDeleteAsync(ct);
        }
    }
}
