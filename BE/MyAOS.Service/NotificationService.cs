using MyAOS.Domain.Dto;
using MyAOS.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MyAOS.Service
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepo;

        public NotificationService(INotificationRepository notificationRepo)
        {
            _notificationRepo = notificationRepo;
        }

        public async Task<List<NotificationDto>> GetUserNotificationsAsync(Guid tenantId, Guid userId, CancellationToken ct = default)
        {
            var notifications = await _notificationRepo.GetUserNotificationsAsync(tenantId, userId, 50, ct);

            return notifications.Select(n =>
            {
                var subject = n.Notification.Subject;
                if (subject != null)
                {
                    if (subject.StartsWith("Restricted:"))
                    {
                        subject = subject.Substring("Restricted:".Length);
                    }
                    else if (subject.StartsWith("[Restricted]"))
                    {
                        subject = subject.Substring("[Restricted]".Length);
                    }
                }
                return new NotificationDto
                {
                    Id = n.Notification.Id,
                    Subject = subject ?? string.Empty,
                    Body = n.Notification.Body,
                    SentAt = n.Notification.SentAt,
                    IsRead = n.IsRead
                };
            }).ToList();
        }

        public async Task MarkAsReadAsync(Guid userId, Guid notificationId, CancellationToken ct = default)
        {
            await _notificationRepo.MarkAsReadAsync(userId, notificationId, ct);
            await _notificationRepo.SaveChangesAsync(ct);
        }

        public async Task MarkAllAsReadAsync(Guid tenantId, Guid userId, CancellationToken ct = default)
        {
            await _notificationRepo.MarkAllAsReadAsync(tenantId, userId, ct);
            await _notificationRepo.SaveChangesAsync(ct);
        }
    }
}
