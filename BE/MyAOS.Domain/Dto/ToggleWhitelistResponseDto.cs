using System;
using System.Collections.Generic;
using System.Text;

namespace MyAOS.Domain.Dto
{
    public class ToggleWhitelistResponseDto
    {
        public Guid TenantId { get; set; }

        public bool EmailWhitelistEnabled { get; set; }
    }
}
