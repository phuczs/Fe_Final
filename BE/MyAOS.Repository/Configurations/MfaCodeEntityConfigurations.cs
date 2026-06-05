using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyAOS.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyAOS.Repository.Configurations
{
    public class MfaCodeEntityConfiguration : IEntityTypeConfiguration<MfaCodeEntity>
    {
        public void Configure(EntityTypeBuilder<MfaCodeEntity> builder)
        {
            builder.ToTable("MfaCodes");

            builder.HasKey(m => m.Id);

            builder.Property(m => m.Id)
                .HasDefaultValueSql("NEWSEQUENTIALID()");

            builder.Property(m => m.Code)
                .IsRequired()
                .HasMaxLength(6);

            builder.Property(m => m.CreatedAt)
                .HasDefaultValueSql("SYSUTCDATETIME()");

            builder.HasIndex(m => new { m.UserId, m.Code, m.Used, m.ExpiresAt });
            builder.Property(m => m.Id)
    .ValueGeneratedNever();
        }
    }
}
