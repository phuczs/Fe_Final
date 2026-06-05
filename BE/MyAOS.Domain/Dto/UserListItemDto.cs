using MyAOS.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyAOS.Domain.Dto
{
    public class UserListItemDto
    {
        public Guid Id { get; set; }

        public Guid TenantId { get; set; }

        public string DisplayName { get; set; } = string.Empty;

        public string UserId { get; set; } = string.Empty;

        public string EmailAddress { get; set; } = string.Empty;

        public RoleType Role { get; set; }

        public StatusType Status { get; set; }

        public SignInMethodType SignInMethod { get; set; }

        public DateTime? LastLoginAt { get; set; }

        public string? StaffStudentId { get; set; }

        public string? Sex { get; set; }

        public string? MobilePhone { get; set; }

        public List<int> ProductIds { get; set; } = new();

        public bool MfaEnabled { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
