using MyAOS.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyAOS.Repository
{
    public interface ITenantRepository
    {
        Task<TenantEntity?> GetByIdAsync(
            Guid tenantId,
            CancellationToken ct = default);
        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }
}
