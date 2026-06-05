using System;
using System.Collections.Generic;
using System.Text;

namespace MyAOS.Domain.Dto
{
    public class AuthResponseDto
    {
        public bool MfaRequired { get; init; }

        // Only populated if MFA is required
        public string? TempToken { get; init; }

        // Only populated if login is fully successful
        public string? AccessToken { get; init; }
        public string? RefreshToken { get; init; }
    }
}
