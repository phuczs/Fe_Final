using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyAOS.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyAOS.Repository.Configurations
{
    public class AuditLogEntityConfiguration : IEntityTypeConfiguration<AuditLogEntity>
    {
        public void Configure(EntityTypeBuilder<AuditLogEntity> builder)
        {
            builder.ToTable("AuditLogs");

            builder.HasKey(a => a.Id);
            builder.Property(a => a.Id).UseIdentityColumn();

            builder.Property(a => a.ObjectType).IsRequired().HasMaxLength(100);
            builder.Property(a => a.Action).IsRequired().HasMaxLength(50);
            builder.Property(a => a.TargetName).HasMaxLength(256);
            builder.Property(a => a.ActorEmail).HasMaxLength(256);
            builder.Property(a => a.ChangeDetail).HasColumnType("nvarchar(max)");

            builder.Property(a => a.OccurredAt).HasDefaultValueSql("SYSUTCDATETIME()");

            // AuditLogs are read-only from EF perspective; no navigation mutations.
            builder.HasIndex(a => new { a.TenantId, a.OccurredAt });
            builder.HasIndex(a => a.TargetUserId)
                   .HasFilter("[TargetUserId] IS NOT NULL");
        }
    }
}
