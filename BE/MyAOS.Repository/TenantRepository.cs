using Microsoft.EntityFrameworkCore;
using MyAOS.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyAOS.Repository
{
    public class TenantRepository : DatabaseRepositoryBase<TenantEntity>, ITenantRepository
    {
        public TenantRepository(AppDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<TenantEntity?> GetByIdAsync(
           Guid tenantId,
           CancellationToken ct = default)
        {
            return await _dbSet
                .FirstOrDefaultAsync(x => x.Id == tenantId, ct);
        }
        public Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            return _context.SaveChangesAsync(ct);
        }
    }
}
