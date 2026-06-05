using MyAOS.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyAOS.Domain.Dto
{
    public class CreateUsersRequest
    {
        public List<CreateUserItemRequest> Users { get; set; } = new();
    }

    public class CreateUserItemRequest
    {
        public string UserId { get; set; } = string.Empty;

        public string EmailAddress { get; set; } = string.Empty;

        public string DisplayName { get; set; } = string.Empty;

        public string? StaffStudentId { get; set; }

        public string? Sex { get; set; }

        public string? MobilePhone { get; set; }

        public RoleType Role { get; set; } = RoleType.TenantUser;

        public SignInMethodType SignInMethod { get; set; } = SignInMethodType.Local;

        public List<int> ProductIds { get; set; } = new();
    }
}
