using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyAOS.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyAOS.Repository.Configurations
{
    namespace MyAOS.Repository.Configurations
    {
        public class RefreshTokenEntityConfiguration : IEntityTypeConfiguration<RefreshTokenEntity>
        {
            public void Configure(EntityTypeBuilder<RefreshTokenEntity> builder)
            {
                builder.ToTable("RefreshTokens");

                builder.HasKey(x => x.Id);

                builder.Property(x => x.Id)
                    .HasDefaultValueSql("NEWSEQUENTIALID()");

                builder.Property(x => x.UserId)
                    .IsRequired();

                builder.Property(x => x.TokenHash)
                    .IsRequired()
                    .HasMaxLength(128);

                builder.Property(x => x.ReplacedByTokenHash)
                    .HasMaxLength(128);

                builder.Property(x => x.CreatedAt)
                    .HasDefaultValueSql("SYSUTCDATETIME()");

                builder.HasIndex(x => x.TokenHash)
                    .IsUnique();

                builder.HasIndex(x => new { x.UserId, x.ExpiresAt });

                builder.HasOne(x => x.User)
                    .WithMany(u => u.RefreshTokens)
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            }
        }
    }
}
