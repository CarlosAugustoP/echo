using EchoProject.Domain.Common;
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
                .WithMessage("Um e-mail válido é obrigatório.");

            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(8)
                .WithMessage("A senha deve ter pelo menos 8 caracteres.")
                .Must(x=> x.Any(char.IsDigit) && x.Any(char.IsLetter) && x.Any(c => char.IsSymbol(c) || char.IsPunctuation(c)))
                .WithMessage("A senha deve conter pelo menos uma letra, um número e um caractere especial.");
        }
    }
}
