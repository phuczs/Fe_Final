using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MyAOS.Domain.Entity;

namespace MyAOS.Repository
{
    public static class UserRepositoryExtensions
    {
        public static async Task<UserEntity?> GetByIdAsync(this IUserRepository repo, Guid tenantId, Guid userId, CancellationToken ct = default)
        {
            if (repo == null) throw new ArgumentNullException(nameof(repo));

            var users = await repo.GetByIdsAsync(tenantId, new[] { userId }, ct);
            return users?.FirstOrDefault();
        }
    }
}
