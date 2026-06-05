using Microsoft.EntityFrameworkCore;
using MyAOS.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyAOS.Repository
{
    public class ProductRepository : DatabaseRepositoryBase<ProductEntity>, IProductRepository
    {
        public ProductRepository(AppDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<List<ProductEntity>> GetActiveByIdsAsync(
            IEnumerable<int> productIds,
            CancellationToken ct = default)
        {
            var ids = productIds.Distinct().ToList();

            return await _dbSet
                .AsNoTracking()
                .Where(p => ids.Contains(p.Id) && p.IsActive)
                .ToListAsync(ct);
        }
    }
}
