using System;
using System.Collections.Generic;
using System.Text;

namespace MyAOS.Domain.Dto
{
    public class ResetMyPasswordRequest
    {
        public string CurrentPassword { get; set; } = string.Empty;

        public string NewPassword { get; set; } = string.Empty;

        public string ConfirmNewPassword { get; set; } = string.Empty;
    }
}
