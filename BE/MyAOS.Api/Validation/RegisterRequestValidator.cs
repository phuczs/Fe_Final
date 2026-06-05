using FluentValidation;
using MyAOS.Domain.Dto;

namespace MyAOS.Api.Validation
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty()
                .WithMessage("User ID is required.")
                .MaximumLength(100)
                .WithMessage("User ID must not exceed 100 characters.")
                .Matches("^[a-zA-Z0-9._-]+$")
                .WithMessage("User ID can only contain letters, numbers, dot, underscore, or hyphen.");

            RuleFor(x => x.EmailAddress)
                .NotEmpty()
                .WithMessage("Email address is required.")
                .EmailAddress()
                .WithMessage("Email address format is invalid.")
                .MaximumLength(256);

            RuleFor(x => x.FullName)
                .NotEmpty()
                .WithMessage("Full name is required.")
                .MaximumLength(200)
                .WithMessage("Full name must not exceed 200 characters.");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password is required.")
                .MinimumLength(8)
                .WithMessage("Password must be at least 8 characters.")
                .Matches("[A-Z]")
                .WithMessage("Password must contain at least one uppercase letter.")
                .Matches("[a-z]")
                .WithMessage("Password must contain at least one lowercase letter.")
                .Matches("[0-9]")
                .WithMessage("Password must contain at least one number.");

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty()
                .WithMessage("Confirm password is required.")
                .Equal(x => x.Password)
                .WithMessage("Confirm password does not match password.");

            RuleFor(x => x.CountryCode)
                .NotEmpty()
                .WithMessage("Country code is required.")
                .MaximumLength(10);

            RuleFor(x => x.PhoneNumber)
                .NotEmpty()
                .WithMessage("Phone number is required.")
                .MaximumLength(30)
                .WithMessage("Phone number must not exceed 30 characters.")
                .Matches("^[0-9]+$")
                .WithMessage("Phone number must contain digits only.");

            //RuleFor(x => x.SecurityVerificationCode)
            //    .NotEmpty()
            //    .WithMessage("Security verification code is required.")
            //    .Length(4, 6)
            //    .WithMessage("Security verification code must be between 4 and 6 characters.")
            //    .Matches("^[0-9]+$")
            //    .WithMessage("Security verification code must contain digits only.");
        }
    }
}
