using MyAOS.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyAOS.Domain.Dto
{
    public class UserDto
    {
        public Guid Id { get; init; }
        public string Email { get; init; } = string.Empty;
        public string FullName { get; init; } = string.Empty;
        public RoleType Role { get; init; }
        public StatusType Status { get; init; }
        public SignInMethodType SignInMethod { get; init; }
        public bool MfaEnabled { get; init; }
        public DateTime CreatedAt { get; init; }
    }
}
