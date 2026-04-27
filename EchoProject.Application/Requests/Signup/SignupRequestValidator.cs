using EchoProject.Domain.Common;
using EchoProject.Domain.UserAggregate;
using FluentValidation;

namespace EchoProject.Application.Requests.Signup
{
    
    public class SignupRequestValidator : AbstractValidator<SignupRequest>
    { 
        public SignupRequestValidator()
        {
            RuleFor(x => x.Name)
                .MaximumLength(100)
                .NotEmpty()
                .WithMessage("O nome é obrigatório.");
            
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .WithMessage("Um e-mail válido é obrigatório.");

            RuleFor(x => x.Role)
                .Must(x => x != UserRole.EchoAdmin)
                .WithMessage("O perfil deve ser Doador ou ONG.");

                RuleFor(x => x.Password)
                    .NotEmpty()
                    .MinimumLength(8)
                    .WithMessage("A senha deve ter pelo menos 8 caracteres.")
                    .Must(x=> x.Any(char.IsDigit) && x.Any(char.IsLetter) && x.Any(c => char.IsSymbol(c) || char.IsPunctuation(c)))
                    .WithMessage("A senha deve conter pelo menos uma letra, um número e um símbolo.");

            RuleFor(x => x.TaxId)
                .NotEmpty()
                .WithMessage("O documento fiscal é obrigatório.")
                .Must(x => Helpers.ValidTaxId(x))
                .WithMessage("O documento fiscal deve ser válido.");
            
            RuleFor(x => x.WalletAddress)
                .NotEmpty()
                .WithMessage("O endereço da carteira é obrigatório.");

            RuleFor(x => x.Address).NotNull().WithMessage("O endereço é obrigatório.")
                .ChildRules(address =>
                {
                    address.RuleFor(a => a.Street)
                        .NotEmpty()
                        .WithMessage("A rua é obrigatória.");
                    
                    address.RuleFor(a => a.City)
                        .NotEmpty()
                        .WithMessage("A cidade é obrigatória.")
                        .Must(a => a.All(char.IsLetter) || a.All(c => char.IsLetter(c) || char.IsWhiteSpace(c)))
                        .WithMessage("A cidade é obrigatória.");
                    
                    address.RuleFor(a => a.State)
                        .NotEmpty()
                        .WithMessage("O estado é obrigatório.")
                        .Must(a => a.Length == 2 && a.All(char.IsLetter))
                        .WithMessage("O estado deve ser uma sigla de 2 letras.");
                    
                    address.RuleFor(a => a.ZipCode)
                        .NotEmpty()
                        .WithMessage("O CEP é obrigatório.")
                        .Must(a => a.All(char.IsDigit));
                    
                    address.RuleFor(a => a.CountryCode)
                        .NotEmpty()
                        .WithMessage("O código do país é obrigatório.")
                        .Must(a => a.Length == 2 && a.All(char.IsLetter))
                        .WithMessage("O código do país deve ser um código ISO de 2 letras.");
                });
        }
    }
}
