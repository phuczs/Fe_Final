using Microsoft.EntityFrameworkCore;
using MyAOS.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyAOS.Repository
{
    public interface IRefreshTokenRepository
    {
        Task AddAsync(RefreshTokenEntity entity, CancellationToken ct = default);
        Task<RefreshTokenEntity?> GetByTokenHashAsync(string tokenHash, CancellationToken ct = default);
        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }

    public class RefreshTokenRepository : DatabaseRepositoryBase<RefreshTokenEntity>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<RefreshTokenEntity?> GetByTokenHashAsync(string tokenHash, CancellationToken ct = default)
        {
            return await _dbSet
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.TokenHash == tokenHash, ct);
        }
    }
}
