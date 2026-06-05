namespace MyAOS.Domain.Dto
{
    public class VerificationCodeResponse
    {
        public bool Success { get; set; }

        public string Message { get; set; } = string.Empty;

        public string? CodeSent { get; set; }  // "email" or "sms" - null for verification response

        public string? Code { get; set; }  // For testing purposes only
    }
}
