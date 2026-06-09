using Microsoft.EntityFrameworkCore;
using MyAOS.Domain.Entity;
using System;
using System.Collections.Generic;

namespace MyAOS.Repository
{
    public class FavouriteProductRepository : DatabaseRepositoryBase<FavouriteProductEntity>, IFavouriteProductRepository
    {
        public FavouriteProductRepository(AppDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<List<FavouriteProductEntity>> GetByUserIdAsync(
            Guid userId,
            CancellationToken ct = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(f => f.UserId == userId)
                .OrderBy(f => f.AddedAt)
                .ToListAsync(ct);
        }

        public async Task<FavouriteProductEntity?> GetAsync(
            Guid userId,
            int productId,
            CancellationToken ct = default)
        {
            return await _dbSet
                .FirstOrDefaultAsync(f => f.UserId == userId && f.ProductId == productId, ct);
        }

        public async Task AddAsync(FavouriteProductEntity entity, CancellationToken ct = default)
        {
            await _dbSet.AddAsync(entity, ct);
        }

        public Task RemoveAsync(FavouriteProductEntity entity, CancellationToken ct = default)
        {
            _dbSet.Remove(entity);
            return Task.CompletedTask;
        }
    }
}
