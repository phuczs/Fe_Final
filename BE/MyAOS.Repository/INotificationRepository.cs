using MyAOS.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MyAOS.Repository
{
    public interface INotificationRepository
    {
        Task<NotificationEntity> AddAsync(NotificationEntity notification, CancellationToken ct = default);
        Task<List<(NotificationEntity Notification, bool IsRead)>> GetUserNotificationsAsync(Guid tenantId, Guid userId, int top = 50, CancellationToken ct = default);
        Task MarkAsReadAsync(Guid userId, Guid notificationId, CancellationToken ct = default);
        Task MarkAllAsReadAsync(Guid tenantId, Guid userId, CancellationToken ct = default);
        Task SaveChangesAsync(CancellationToken ct = default);
    }
}
