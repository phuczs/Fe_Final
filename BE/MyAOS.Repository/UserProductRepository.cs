using Microsoft.EntityFrameworkCore;
using MyAOS.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyAOS.Repository
{
    public class UserProductRepository : DatabaseRepositoryBase<UserProductEntity>, IUserProductRepository
    {
        public UserProductRepository(AppDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<List<UserProductEntity>> GetByUserIdAsync(
            Guid userId,
            CancellationToken ct = default)
        {
            return await _dbSet
                .Where(x => x.UserId == userId)
                .ToListAsync(ct);
        }

        public async Task ReplaceUserProductsAsync(
            Guid userId,
            IEnumerable<int> productIds,
            CancellationToken ct = default)
        {
            var currentItems = await _dbSet
                .Where(x => x.UserId == userId)
                .ToListAsync(ct);

            _dbSet.RemoveRange(currentItems);

            var newItems = productIds
                .Distinct()
                .Select(productId => new UserProductEntity
                {
                    UserId = userId,
                    ProductId = productId
                })
                .ToList();

            await _dbSet.AddRangeAsync(newItems, ct);
        }

        public Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            return _context.SaveChangesAsync(ct);
        }
    }
}
