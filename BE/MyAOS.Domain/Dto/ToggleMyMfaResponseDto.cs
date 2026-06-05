using System;
using System.Collections.Generic;
using System.Text;

namespace MyAOS.Domain.Dto
{
    public class ToggleMyMfaResponseDto
    {
        public Guid UserId { get; set; }

        public bool MfaEnabled { get; set; }
    }
}
