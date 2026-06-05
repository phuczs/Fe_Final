using System;
using System.Collections.Generic;
using System.Text;

namespace MyAOS.Domain.Dto
{
    public class RegisterResponseDto
    {
        public Guid UserId { get; set; }
        public Guid TenantId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
    }
}
