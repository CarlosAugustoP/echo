using FluentValidation;

namespace EchoProject.Application.Requests.Login
{
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
           RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .WithMessage("Valid email is required.");

            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(8)
                .WithMessage("Password must be at least 8 characters long.")
                .Must(x=> x.Any(char.IsDigit) && x.Any(char.IsLetter) && x.Any(char.IsSymbol))
                .WithMessage("Password must contain at least one letter, one number and one symbol.");
        }
    }
}