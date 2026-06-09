using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyAOS.Domain.Entity;
using System;

namespace MyAOS.Repository.Configurations
{
    public class NotificationEntityConfiguration : IEntityTypeConfiguration<NotificationEntity>
    {
        public void Configure(EntityTypeBuilder<NotificationEntity> builder)
        {
            builder.ToTable("Notifications");

            builder.HasKey(n => n.Id);
            // We use Guid as PK, no need for UseIdentityColumn.

            builder.Property(n => n.Subject).IsRequired().HasMaxLength(256);
            builder.Property(n => n.Body).IsRequired().HasColumnType("nvarchar(max)");
            builder.Property(n => n.SentAt).HasDefaultValueSql("SYSUTCDATETIME()");

            builder.HasIndex(n => new { n.TenantId, n.SentAt });

            // Navigation
            builder.HasOne(n => n.Tenant)
                   .WithMany()
                   .HasForeignKey(n => n.TenantId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
