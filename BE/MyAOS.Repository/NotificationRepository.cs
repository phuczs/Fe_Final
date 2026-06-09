using Microsoft.EntityFrameworkCore;
using MyAOS.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MyAOS.Repository
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly AppDbContext _context;

        public NotificationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<NotificationEntity> AddAsync(NotificationEntity notification, CancellationToken ct = default)
        {
            await _context.Notifications.AddAsync(notification, ct);
            return notification;
        }

        public async Task<List<(NotificationEntity Notification, bool IsRead)>> GetUserNotificationsAsync(Guid tenantId, Guid userId, int top = 50, CancellationToken ct = default)
        {
            var tenant = await _context.Tenants.FirstOrDefaultAsync(t => t.Id == tenantId, ct);
            var whitelistEnabled = tenant?.EmailWhitelistEnabled ?? false;

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId, ct);
            bool isWhitelisted = false;
            if (user != null)
            {
                isWhitelisted = await _context.EmailWhitelist.AnyAsync(w => w.TenantId == tenantId && w.Email == user.Email, ct);
            }

            if (whitelistEnabled && !isWhitelisted)
            {
                return new List<(NotificationEntity Notification, bool IsRead)>();
            }

            var query = from n in _context.Notifications
                        where n.TenantId == tenantId
                        join s in _context.UserNotificationStates
                             on new { NotificationId = n.Id, UserId = userId } equals new { s.NotificationId, s.UserId } into states
                        from s in states.DefaultIfEmpty()
                        orderby n.SentAt descending
                        select new { Notification = n, IsRead = s != null && s.IsRead };

            if (!isWhitelisted)
            {
                query = query.Where(q => !q.Notification.Subject.StartsWith("Restricted:")
                                      && !EF.Functions.Like(q.Notification.Subject, "[[]Restricted]%"));
            }

            var results = await query.Take(top).ToListAsync(ct);
            return results.Select(r => (r.Notification, r.IsRead)).ToList();
        }

        public async Task MarkAsReadAsync(Guid userId, Guid notificationId, CancellationToken ct = default)
        {
            var state = await _context.UserNotificationStates
                .FirstOrDefaultAsync(s => s.UserId == userId && s.NotificationId == notificationId, ct);

            if (state == null)
            {
                state = new UserNotificationStateEntity
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    NotificationId = notificationId,
                    IsRead = true,
                    ReadAt = DateTime.UtcNow
                };
                await _context.UserNotificationStates.AddAsync(state, ct);
            }
            else if (!state.IsRead)
            {
                state.IsRead = true;
                state.ReadAt = DateTime.UtcNow;
            }
        }

        public async Task MarkAllAsReadAsync(Guid tenantId, Guid userId, CancellationToken ct = default)
        {
            var tenant = await _context.Tenants.FirstOrDefaultAsync(t => t.Id == tenantId, ct);
            var whitelistEnabled = tenant?.EmailWhitelistEnabled ?? false;

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId, ct);
            bool isWhitelisted = false;
            if (user != null)
            {
                isWhitelisted = await _context.EmailWhitelist.AnyAsync(w => w.TenantId == tenantId && w.Email == user.Email, ct);
            }

            if (whitelistEnabled && !isWhitelisted)
            {
                return;
            }

            // Get all notifications for the tenant
            var query = _context.Notifications.Where(n => n.TenantId == tenantId);
            if (!isWhitelisted)
            {
                query = query.Where(n => !n.Subject.StartsWith("Restricted:")
                                      && !EF.Functions.Like(n.Subject, "[[]Restricted]%"));
            }

            var notifications = await query
                .Select(n => n.Id)
                .ToListAsync(ct);

            var existingStates = await _context.UserNotificationStates
                .Where(s => s.UserId == userId && notifications.Contains(s.NotificationId))
                .ToListAsync(ct);

            var existingStateMap = existingStates.ToDictionary(s => s.NotificationId);

            foreach (var nId in notifications)
            {
                if (!existingStateMap.TryGetValue(nId, out var state))
                {
                    _context.UserNotificationStates.Add(new UserNotificationStateEntity
                    {
                        Id = Guid.NewGuid(),
                        UserId = userId,
                        NotificationId = nId,
                        IsRead = true,
                        ReadAt = DateTime.UtcNow
                    });
                }
                else if (!state.IsRead)
                {
                    state.IsRead = true;
                    state.ReadAt = DateTime.UtcNow;
                }
            }
        }

        public async Task SaveChangesAsync(CancellationToken ct = default)
        {
            await _context.SaveChangesAsync(ct);
        }
    }
}
