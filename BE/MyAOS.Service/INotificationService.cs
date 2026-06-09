using MyAOS.Domain.Dto;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MyAOS.Service
{
    public interface INotificationService
    {
        Task<List<NotificationDto>> GetUserNotificationsAsync(Guid tenantId, Guid userId, CancellationToken ct = default);
        Task MarkAsReadAsync(Guid userId, Guid notificationId, CancellationToken ct = default);
        Task MarkAllAsReadAsync(Guid tenantId, Guid userId, CancellationToken ct = default);
    }

    public class NotificationDto
    {
        public Guid Id { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
        public bool IsRead { get; set; }
    }
}
