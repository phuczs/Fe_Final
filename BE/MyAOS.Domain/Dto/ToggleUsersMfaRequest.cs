using System;
using System.Collections.Generic;
using System.Text;

namespace MyAOS.Domain.Dto
{
    public class ToggleUsersMfaRequest
    {
        public List<Guid> UserIds { get; set; } = new();

        public bool Enabled { get; set; }
    }
}
