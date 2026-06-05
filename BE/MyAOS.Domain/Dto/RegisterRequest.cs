namespace MyAOS.Domain.Dto
{
    public class RegisterRequest
    {
        public string UserId { get; set; } = string.Empty;

        public string EmailAddress { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string ConfirmPassword { get; set; } = string.Empty;

        public string CountryCode { get; set; } = "+65";

        public string PhoneNumber { get; set; } = string.Empty;

        public string SecurityVerificationCode { get; set; } = string.Empty;
    }
}
