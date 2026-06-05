using MyAOS.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyAOS.Repository
{
    public interface IUserProductRepository
    {
        Task<List<UserProductEntity>> GetByUserIdAsync(
            Guid userId,
            CancellationToken ct = default);

        Task ReplaceUserProductsAsync(
            Guid userId,
            IEnumerable<int> productIds,
            CancellationToken ct = default);

        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }
}
