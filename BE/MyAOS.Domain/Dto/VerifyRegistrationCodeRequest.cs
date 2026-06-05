namespace MyAOS.Domain.Dto
{
    public class VerifyRegistrationCodeRequest
    {
        public string EmailAddress { get; set; } = string.Empty;

        public string VerificationCode { get; set; } = string.Empty;
    }
}
