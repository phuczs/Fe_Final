using System;

namespace MyAOS.Domain.Entity
{
    public class UserNotificationStateEntity
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid NotificationId { get; set; }
        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }

        public UserEntity User { get; set; } = null!;
        public NotificationEntity Notification { get; set; } = null!;
    }
}
