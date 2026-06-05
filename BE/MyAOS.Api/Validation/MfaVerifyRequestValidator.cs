using FluentValidation;
using MyAOS.Domain.Dto;

namespace MyAOS.Api.Validation
{
    public class MfaVerifyRequestValidator : AbstractValidator<MfaVerifyRequest>
    {
        public MfaVerifyRequestValidator()
        {
            RuleFor(x => x.TempToken)
                .NotEmpty()
                .WithMessage("Temp token is required.");

            RuleFor(x => x.Code)
                .NotEmpty()
                .WithMessage("MFA code is required.")
                .Matches(@"^\d{4}(\d{2})?$")
                .WithMessage("MFA code must be 4 or 6 digits.");
        }
    }
}
