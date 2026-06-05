using MyAOS.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace MyAOS.Repository
{
    public interface IRegistrationVerificationCodeRepository
    {
        Task AddAsync(RegistrationVerificationCodeEntity code, CancellationToken ct = default);
        Task<RegistrationVerificationCodeEntity?> GetByEmailAsync(Guid tenantId, string email, CancellationToken ct = default);
        Task<RegistrationVerificationCodeEntity?> GetValidCodeAsync(Guid tenantId, string email, string code, CancellationToken ct = default);
        Task<int> SaveChangesAsync(CancellationToken ct = default);
        Task InvalidatePreviousCodesAsync(
    Guid tenantId,
    string email,
    CancellationToken ct = default);
    }

    public class RegistrationVerificationCodeRepository : DatabaseRepositoryBase<RegistrationVerificationCodeEntity>, IRegistrationVerificationCodeRepository
    {
        public RegistrationVerificationCodeRepository(AppDbContext context) : base(context) { }

        public async Task<RegistrationVerificationCodeEntity?> GetByEmailAsync(Guid tenantId, string email, CancellationToken ct = default)
        {
            return await _dbSet.FirstOrDefaultAsync(vc => vc.TenantId == tenantId && vc.Email == email.ToLowerInvariant(), ct);
        }

        public async Task<RegistrationVerificationCodeEntity?> GetValidCodeAsync(Guid tenantId, string email, string code, CancellationToken ct = default)
        {
            var now = DateTime.UtcNow;

            return await _dbSet.FirstOrDefaultAsync(vc =>
                vc.TenantId == tenantId &&
                vc.Email == email.ToLowerInvariant() &&
                vc.Code == code &&
                vc.Verified == false &&
                vc.ExpiresAt > now, ct);
        }
        public async Task InvalidatePreviousCodesAsync(
    Guid tenantId,
    string email,
    CancellationToken ct = default)
        {
            var normalizedEmail = email.Trim().ToLowerInvariant();

            await _dbSet
                .Where(x =>
                    x.TenantId == tenantId &&
                    x.Email == normalizedEmail &&
                    x.Verified == false)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(x => x.Verified, true),
                    ct);
        }

    }
}
