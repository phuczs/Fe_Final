using System;
using System.Collections.Generic;
using System.Text;

namespace MyAOS.Domain.Dto
{
    public class WhitelistSettingsDto
    {
        public Guid TenantId { get; set; }

        public bool EmailWhitelistEnabled { get; set; }

        public List<WhitelistEmailDto> Emails { get; set; } = new();
    }

    public class WhitelistEmailDto
    {
        public Guid Id { get; set; }

        public string Email { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}
