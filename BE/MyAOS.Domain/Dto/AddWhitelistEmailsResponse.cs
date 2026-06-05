using System;
using System.Collections.Generic;
using System.Text;

namespace MyAOS.Domain.Dto
{
    public class AddWhitelistEmailsResponseDto
    {
        public Guid TenantId { get; set; }

        public List<string> AddedEmails { get; set; } = new();

        public List<string> SkippedEmails { get; set; } = new();
    }
}
