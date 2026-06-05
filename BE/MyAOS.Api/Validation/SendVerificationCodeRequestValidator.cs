using FluentValidation;
using MyAOS.Domain.Dto;

namespace MyAOS.Api.Validation
{
    public class SendVerificationCodeRequestValidator : AbstractValidator<SendVerificationCodeRequest>
    {
        public SendVerificationCodeRequestValidator()
        {
            RuleFor(x => x.EmailAddress)
                .NotEmpty()
                .WithMessage("Email address is required.")
                .EmailAddress()
                .WithMessage("Email address format is invalid.")
                .MaximumLength(256);


            RuleFor(x => x.PhoneNumber)
                .NotEmpty()
                .WithMessage("Phone number is required.")
                .MaximumLength(30)
                .WithMessage("Phone number must not exceed 30 characters.")
                .Matches("^[0-9]+$")
                .WithMessage("Phone number must contain digits only.");
        }
    }
}
