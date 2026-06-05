using System;
using System.Collections.Generic;
using System.Text;

namespace MyAOS.Domain.Dto
{
    public class ToggleUsersMfaResponseDto
    {
        public bool Enabled { get; set; }

        public int UpdatedCount { get; set; }

        public List<Guid> UpdatedUserIds { get; set; } = new();
    }
}
