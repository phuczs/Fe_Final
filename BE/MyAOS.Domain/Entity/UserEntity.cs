using MyAOS.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyAOS.Domain.Entity
{
    public sealed class UserEntity
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PasswordHash { get; set; }   // null for SSO
        public string FullName { get; set; } = string.Empty;
        public string? PhoneCountryCode { get; set; }
        public string? PhoneNumber { get; set; }
        public string? StaffStudentId { get; set; }
        public string? Sex { get; set; }
        public string? MobilePhone { get; set; }
        public RoleType Role { get; set; }
        public StatusType Status { get; set; }
        public SignInMethodType SignInMethod { get; set; }
        public bool MfaEnabled { get; set; }
        public string? MfaSecret { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation
        public TenantEntity Tenant { get; set; } = null!;
        public ICollection<UserProductEntity> UserProducts { get; set; } = new List<UserProductEntity>();
        public ICollection<FavouriteProductEntity> FavoriteProducts { get; set; } = new List<FavouriteProductEntity>();
        public ICollection<RefreshTokenEntity> RefreshTokens { get; set; } = new List<RefreshTokenEntity>();
        public ICollection<MfaCodeEntity> MfaCodes { get; set; } = new List<MfaCodeEntity>();
    }
}
