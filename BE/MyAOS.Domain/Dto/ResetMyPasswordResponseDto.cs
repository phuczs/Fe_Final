using System;
using System.Collections.Generic;
using System.Text;

namespace MyAOS.Domain.Dto
{
    public class ResetMyPasswordResponseDto
    {
        public bool Success { get; set; }

        public string Message { get; set; } = string.Empty;
    }

}
