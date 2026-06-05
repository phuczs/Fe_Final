namespace MyAOS.Domain.Dto
{
    public class SendVerificationCodeRequest
    {
        public string EmailAddress { get; set; } = string.Empty;

        

        public string PhoneNumber { get; set; } = string.Empty;
    }
}
