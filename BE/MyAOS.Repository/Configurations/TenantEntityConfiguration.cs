using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyAOS.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyAOS.Repository.Configurations
{
    public class TenantEntityConfiguration : IEntityTypeConfiguration<TenantEntity>
    {
        public void Configure(EntityTypeBuilder<TenantEntity> builder)
        {
            builder.ToTable("Tenants", "mos", tb =>
            {
                tb.UseSqlOutputClause(false);
            });

            builder.HasKey(t => t.Id);
            builder.Property(t => t.Id)
                   .HasDefaultValueSql("NEWSEQUENTIALID()");

            builder.Property(t => t.Name)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(t => t.EmailWhitelistEnabled)
                   .HasDefaultValue(false);

            builder.Property(t => t.CreatedAt)
                   .HasDefaultValueSql("SYSUTCDATETIME()");

            builder.Property(t => t.UpdatedAt)
                   .HasDefaultValueSql("SYSUTCDATETIME()");

            // Navigation
            builder.HasMany(t => t.Users)
                   .WithOne(u => u.Tenant)
                   .HasForeignKey(u => u.TenantId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
