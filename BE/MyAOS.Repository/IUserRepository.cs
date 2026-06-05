using Microsoft.EntityFrameworkCore;
using MyAOS.Domain.Dto;
using MyAOS.Domain.Entity;
using MyAOS.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MyAOS.Repository
{
    public interface IUserRepository
    {
        Task<UserEntity?> GetByEmailAsync(Guid tenantId, string email, CancellationToken ct = default);
        Task<UserEntity?> GetByUserNameAsync(Guid tenantId, string userName, CancellationToken ct = default);
        Task<List<UserEntity>> GetByIdsAsync(Guid tenantId, IEnumerable<Guid> userIds, CancellationToken ct = default);
        Task<(IEnumerable<UserEntity> Items, int TotalCount)> GetPagedAsync(
     Guid? tenantId,
     GetUsersQuery query,
     CancellationToken ct = default);
        Task BulkDeleteAsync(Guid tenantId, IEnumerable<Guid> userIds, CancellationToken ct = default);
        Task DeleteRangeAsync(IEnumerable<UserEntity> users,CancellationToken ct = default);
        Task AddAsync(UserEntity user, CancellationToken ct = default);
        Task<int> SaveChangesAsync(CancellationToken ct = default);
        Task<UserEntity?> GetByIdAsync(
    Guid tenantId,
    Guid userId,
    CancellationToken ct = default);

    }

    public class UserRepository : DatabaseRepositoryBase<UserEntity>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context) { }

        public async Task<UserEntity?> GetByEmailAsync(Guid tenantId, string email, CancellationToken ct = default)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.TenantId == tenantId && u.Email == email, ct);
        }

        public async Task<UserEntity?> GetByUserNameAsync(Guid tenantId, string userName, CancellationToken ct = default)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.TenantId == tenantId && u.UserName == userName, ct);
        }
        public async Task<UserEntity?> GetByIdAsync(
    Guid tenantId,
    Guid userId,
    CancellationToken ct = default)
        {
            return await _dbSet
                .FirstOrDefaultAsync(u => u.TenantId == tenantId && u.Id == userId, ct);
        }

        public async Task<List<UserEntity>> GetByIdsAsync(
    Guid tenantId,
    IEnumerable<Guid> userIds,
    CancellationToken ct = default)
        {
            var ids = userIds.ToList();

            return await _dbSet
                .Where(u => u.TenantId == tenantId && ids.Contains(u.Id))
                .ToListAsync(ct);
            
        }

        public Task DeleteRangeAsync(
            IEnumerable<UserEntity> users,
            CancellationToken ct = default)
        {
            _dbSet.RemoveRange(users);
            return Task.CompletedTask;
        }

        public async Task<(IEnumerable<UserEntity> Items, int TotalCount)> GetPagedAsync(
    Guid? tenantId,
    GetUsersQuery request,
    CancellationToken ct = default)
        {
            var query = _dbSet
                .AsNoTracking()
                .Include(u => u.UserProducts)
                    .ThenInclude(up => up.Product)
                .AsQueryable();

            if (tenantId.HasValue)
            {
                query = query.Where(u => u.TenantId == tenantId.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var keyword = request.Search.Trim();

                query = query.Where(u =>
                    u.FullName.Contains(keyword) ||
                    u.UserName.Contains(keyword) ||
                    u.Email.Contains(keyword) ||
                    (u.StaffStudentId != null && u.StaffStudentId.Contains(keyword)));
            }

            if (!string.IsNullOrWhiteSpace(request.Role))
            {
                var roleValue = request.Role.Trim();

                if (Enum.TryParse<RoleType>(roleValue, true, out var roleEnum))
                {
                    query = query.Where(u => u.Role == roleEnum);
                }
                else if (byte.TryParse(roleValue, out var roleNumber))
                {
                    query = query.Where(u => (byte)u.Role == roleNumber);
                }
            }

            if (!string.IsNullOrWhiteSpace(request.Status))
            {
                var statusValue = request.Status.Trim();

                if (Enum.TryParse<StatusType>(statusValue, true, out var statusEnum))
                {
                    query = query.Where(u => u.Status == statusEnum);
                }
                else if (byte.TryParse(statusValue, out var statusNumber))
                {
                    query = query.Where(u => (byte)u.Status == statusNumber);
                }
            }

            if (!string.IsNullOrWhiteSpace(request.SignInMethod))
            {
                var signInMethodValue = request.SignInMethod.Trim();

                if (Enum.TryParse<SignInMethodType>(signInMethodValue, true, out var signInMethodEnum))
                {
                    query = query.Where(u => u.SignInMethod == signInMethodEnum);
                }
                else if (byte.TryParse(signInMethodValue, out var signInMethodNumber))
                {
                    query = query.Where(u => (byte)u.SignInMethod == signInMethodNumber);
                }
            }

            if (request.ProductId.HasValue)
            {
                query = query.Where(u =>
                    u.UserProducts.Any(up => up.ProductId == request.ProductId.Value));
            }

            var page = request.Page <= 0 ? 1 : request.Page;
            var pageSize = request.PageSize <= 0 ? 10 : request.PageSize;
            if (pageSize > 100) pageSize = 100;

            var descending = string.Equals(
                request.SortDirection,
                "desc",
                StringComparison.OrdinalIgnoreCase);

            query = request.SortBy?.ToLowerInvariant() switch
            {
                "displayname" or "name" or "fullname" => descending
                    ? query.OrderByDescending(u => u.FullName)
                    : query.OrderBy(u => u.FullName),

                "userid" or "username" => descending
                    ? query.OrderByDescending(u => u.UserName)
                    : query.OrderBy(u => u.UserName),

                "email" => descending
                    ? query.OrderByDescending(u => u.Email)
                    : query.OrderBy(u => u.Email),

                "role" => descending
                    ? query.OrderByDescending(u => u.Role)
                    : query.OrderBy(u => u.Role),

                "status" => descending
                    ? query.OrderByDescending(u => u.Status)
                    : query.OrderBy(u => u.Status),

                "signinmethod" => descending
                    ? query.OrderByDescending(u => u.SignInMethod)
                    : query.OrderBy(u => u.SignInMethod),

                "lastlogin" or "lastloginat" => descending
                    ? query.OrderByDescending(u => u.LastLoginAt)
                    : query.OrderBy(u => u.LastLoginAt),

                "createdat" => descending
                    ? query.OrderByDescending(u => u.CreatedAt)
                    : query.OrderBy(u => u.CreatedAt),

                _ => query.OrderBy(u => u.FullName)
            };

            var totalCount = await query.CountAsync(ct);

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return (items, totalCount);
        }

        public async Task BulkDeleteAsync(Guid tenantId, IEnumerable<Guid> userIds, CancellationToken ct = default)
        {
            // ExecuteDeleteAsync sends a single bulk DELETE SQL statement directly to the database,
            // bypassing EF Core's memory-heavy change tracker.
            await _dbSet
                .Where(u => u.TenantId == tenantId && userIds.Contains(u.Id))
                .ExecuteDeleteAsync(ct);
        }
    }
}
