using Microsoft.EntityFrameworkCore;
using MyAOS.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyAOS.Repository
{
    public interface IMfaCodeRepository
    {
        Task AddAsync(MfaCodeEntity mfaCode, CancellationToken ct = default);
        Task<MfaCodeEntity?> GetValidCodeAsync(Guid userId, string code, CancellationToken ct = default);
        Task InvalidateAllPreviousCodesAsync(Guid userId, CancellationToken ct = default);
        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }

    public class MfaCodeRepository : DatabaseRepositoryBase<MfaCodeEntity>, IMfaCodeRepository
    {
        public MfaCodeRepository(AppDbContext context) : base(context) { }

        public async Task<MfaCodeEntity?> GetValidCodeAsync(Guid userId, string code, CancellationToken ct = default)
        {
            var now = DateTime.UtcNow;

            // SECURE: Validates the exact constraints matching our SQL Filtered Index.
            return await _dbSet.FirstOrDefaultAsync(m =>
                m.UserId == userId &&
                m.Code == code &&
                m.Used == false &&
                m.ExpiresAt > now, ct);
        }

        public async Task InvalidateAllPreviousCodesAsync(Guid userId, CancellationToken ct = default)
        {
            // Best Practice: Before issuing a new MFA code, immediately invalidate any unused 
            // older codes hanging around for this user to prevent race conditions or confusion.
            await _dbSet
                .Where(m => m.UserId == userId && m.Used == false)
                .ExecuteUpdateAsync(s => s.SetProperty(p => p.Used, true), ct);
        }
    }
}
