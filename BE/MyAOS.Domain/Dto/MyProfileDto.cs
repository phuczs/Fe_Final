using System;
using System.Collections.Generic;
using System.Text;

namespace MyAOS.Domain.Dto
{
    public class MyProfileDto
    {
        public Guid Id { get; set; }

        public Guid TenantId { get; set; }

        public string DisplayName { get; set; } = string.Empty;

        public string UserId { get; set; } = string.Empty;

        public string EmailAddress { get; set; } = string.Empty;

        public string? StaffStudentId { get; set; }

        public string? Sex { get; set; }

        public string? MobilePhone { get; set; }

        public bool MfaEnabled { get; set; }

        public List<int> ProductIds { get; set; } = new();
    }
}
