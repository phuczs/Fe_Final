using MyAOS.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyAOS.Domain.Dto
{
    public class UserCreateRequest
    {
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public RoleType Role { get; set; }

        // Initial password provided by an Admin during user creation
        public string Password { get; set; } = string.Empty;
    }
}
