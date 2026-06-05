using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyAOS.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyAOS.Repository.Configurations
{
    public class RegistrationVerificationCodeEntityConfiguration : IEntityTypeConfiguration<RegistrationVerificationCodeEntity>
    {
        public void Configure(EntityTypeBuilder<RegistrationVerificationCodeEntity> builder)
        {
            builder.ToTable("RegistrationVerificationCodes", "mos", tb =>
            {
                tb.UseSqlOutputClause(false);
            });

            builder.HasKey(vc => vc.Id);
            builder.Property(vc => vc.Id).HasDefaultValueSql("NEWSEQUENTIALID()");

            builder.Property(vc => vc.TenantId).IsRequired();
            builder.Property(vc => vc.Email).IsRequired().HasMaxLength(256);
            builder.Property(vc => vc.PhoneNumber)
                .HasMaxLength(30);
            builder.Property(vc => vc.AttemptCount).HasDefaultValue(0);
            builder.Property(vc => vc.Verified).HasDefaultValue(false);
            builder.Property(vc => vc.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");

            // Index for quick lookup by tenant and email
            builder.HasIndex(vc => new { vc.TenantId, vc.Email });

            // Index for valid codes (not verified)
            builder.HasIndex(vc => new { vc.TenantId, vc.Email, vc.Verified, vc.ExpiresAt })
                .HasFilter("[Verified] = 0");
        }
    }
}
