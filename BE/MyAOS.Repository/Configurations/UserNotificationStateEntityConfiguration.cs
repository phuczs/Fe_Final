using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyAOS.Domain.Entity;
using System;

namespace MyAOS.Repository.Configurations
{
    public class UserNotificationStateEntityConfiguration : IEntityTypeConfiguration<UserNotificationStateEntity>
    {
        public void Configure(EntityTypeBuilder<UserNotificationStateEntity> builder)
        {
            builder.ToTable("UserNotificationStates");

            builder.HasKey(u => u.Id);

            builder.HasIndex(u => new { u.UserId, u.NotificationId }).IsUnique();

            builder.HasOne(u => u.User)
                   .WithMany()
                   .HasForeignKey(u => u.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(u => u.Notification)
                   .WithMany()
                   .HasForeignKey(u => u.NotificationId)
                   .OnDelete(DeleteBehavior.NoAction); // Avoid multiple cascade paths
        }
    }
}
