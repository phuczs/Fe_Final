using Microsoft.EntityFrameworkCore;
using MyAOS.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyAOS.Repository
{
    public sealed class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // ── DbSets ────────────────────────────────────────────────────────────────
        public DbSet<TenantEntity> Tenants => Set<TenantEntity>();
        public DbSet<UserEntity> Users => Set<UserEntity>();
        public DbSet<ProductEntity> Products => Set<ProductEntity>();
        public DbSet<UserProductEntity> UserProducts => Set<UserProductEntity>();
        public DbSet<FavouriteProductEntity> FavoriteProducts => Set<FavouriteProductEntity>();
        public DbSet<EmailWhitelistEntity> EmailWhitelist => Set<EmailWhitelistEntity>();
        public DbSet<AuditLogEntity> AuditLogs => Set<AuditLogEntity>();
        public DbSet<MfaCodeEntity> MfaCodes => Set<MfaCodeEntity>();
        public DbSet<RefreshTokenEntity> RefreshTokens => Set<RefreshTokenEntity>();
        public DbSet<RegistrationVerificationCodeEntity> RegistrationVerificationCodes => Set<RegistrationVerificationCodeEntity>();
        public DbSet<NotificationEntity> Notifications => Set<NotificationEntity>();
        public DbSet<UserNotificationStateEntity> UserNotificationStates => Set<UserNotificationStateEntity>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema("mos");

            // Apply your individual configurations (Users, Tenants, AuditLogs, etc.)
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            // FIX: Define explicit composite primary keys for the link tables

            // 1. Map user product linkages
            modelBuilder.Entity<UserProductEntity>(builder =>
            {
                builder.ToTable("UserProducts");
                builder.HasKey(up => new { up.UserId, up.ProductId }); // Composite PK
            });

            // 2. Map favorite product linkages (Fixes your exact exception)
            modelBuilder.Entity<FavouriteProductEntity>(builder =>
            {
                builder.ToTable("FavoriteProducts");
                builder.HasKey(fp => new { fp.UserId, fp.ProductId }); // Composite PK
            });
        }

    }
}
