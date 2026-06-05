using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyAOS.Domain.Entity;
using MyAOS.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyAOS.Repository.Configurations
{
    public class UserEntityConfiguration : IEntityTypeConfiguration<UserEntity>
    {
        public void Configure(EntityTypeBuilder<UserEntity> builder)
        {
            builder.ToTable("Users", "mos", tb =>
            {
                tb.UseSqlOutputClause(false);
            });

            builder.HasKey(u => u.Id);
            builder.Property(u => u.Id).HasDefaultValueSql("NEWSEQUENTIALID()");

            builder.Property(u => u.UserName).IsRequired().HasMaxLength(100);
            builder.Property(u => u.Email).IsRequired().HasMaxLength(256);
            builder.Property(u => u.PasswordHash).HasMaxLength(512);
            builder.Property(u => u.FullName).IsRequired().HasMaxLength(200);
            builder.Property(u => u.PhoneCountryCode).HasMaxLength(10);
            builder.Property(u => u.PhoneNumber).HasMaxLength(30);
            builder.Property(u => u.StaffStudentId).HasMaxLength(100);
            builder.Property(u => u.Sex).HasMaxLength(20);
            builder.Property(u => u.MobilePhone).HasMaxLength(30);
            builder.Property(u => u.MfaSecret).HasMaxLength(64);

            builder.Property(u => u.Role).HasConversion<byte>().HasDefaultValue(RoleType.TenantUser);
            builder.Property(u => u.Status).HasConversion<byte>().HasDefaultValue(StatusType.Active);
            builder.Property(u => u.SignInMethod).HasConversion<byte>().HasDefaultValue(SignInMethodType.Local);

            builder.Property(u => u.MfaEnabled).HasDefaultValue(false);
            builder.Property(u => u.LastLoginAt);
            builder.Property(u => u.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
            builder.Property(u => u.UpdatedAt).HasDefaultValueSql("SYSUTCDATETIME()");

            // Unique email within tenant
            builder.HasIndex(u => new { u.TenantId, u.Email }).IsUnique();

            // Unique username within tenant
            builder.HasIndex(u => new { u.TenantId, u.UserName }).IsUnique();

            // Navigation
            builder.HasMany(u => u.UserProducts)
                   .WithOne(up => up.User)
                   .HasForeignKey(up => up.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(u => u.FavoriteProducts)
                   .WithOne(fp => fp.User)
                   .HasForeignKey(fp => fp.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(u => u.RefreshTokens)
                   .WithOne(rt => rt.User)
                   .HasForeignKey(rt => rt.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(u => u.MfaCodes)
                   .WithOne(mc => mc.User)
                   .HasForeignKey(mc => mc.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
