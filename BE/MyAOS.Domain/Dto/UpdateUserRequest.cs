using MyAOS.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyAOS.Domain.Dto
{
    public class UpdateUserRequest
    {
        public string? Email { get; set; }

        public string? FullName { get; set; }

        public RoleType? Role { get; set; }

        public StatusType? Status { get; set; }

        public bool? MfaEnabled { get; set; }
    }
}

