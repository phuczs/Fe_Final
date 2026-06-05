using MyAOS.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyAOS.Domain.Dto
{
    public class UpdateUserResponseDto
    {
        public Guid Id { get; set; }

        public Guid TenantId { get; set; }

        public string Email { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public RoleType Role { get; set; }

        public StatusType Status { get; set; }

        public bool MfaEnabled { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
