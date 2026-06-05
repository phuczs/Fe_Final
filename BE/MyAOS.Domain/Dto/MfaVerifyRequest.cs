using System;
using System.Collections.Generic;
using System.Text;

namespace MyAOS.Domain.Dto
{
    public class MfaVerifyRequest
    {
        public string TempToken { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
    }
}
