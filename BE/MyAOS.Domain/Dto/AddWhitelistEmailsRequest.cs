using System;
using System.Collections.Generic;
using System.Text;

namespace MyAOS.Domain.Dto
{
    public class AddWhitelistEmailsRequest
    {
        public List<string> Emails { get; set; } = new();
    }
}
