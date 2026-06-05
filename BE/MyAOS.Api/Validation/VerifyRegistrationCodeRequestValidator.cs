using FluentValidation;
using MyAOS.Domain.Dto;

namespace MyAOS.Api.Validation
{
    public class VerifyRegistrationCodeRequestValidator : AbstractValidator<VerifyRegistrationCodeRequest>
    {
        public VerifyRegistrationCodeRequestValidator()
        {
            RuleFor(x => x.EmailAddress)
                .NotEmpty()
                .WithMessage("Email address is required.")
                .EmailAddress()
                .WithMessage("Email address format is invalid.")
                .MaximumLength(256);

            RuleFor(x => x.VerificationCode)
                .NotEmpty()
                .WithMessage("Verification code is required.")
                .Length(4, 6)
                .WithMessage("Verification code must be between 4 and 6 characters.")
                .Matches("^[0-9]+$")
                .WithMessage("Verification code must contain digits only.");
        }
    }
}
