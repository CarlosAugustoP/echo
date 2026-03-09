using EchoProject.Domain.Common;
using FluentValidation;

namespace EchoProject.Application.Requests.Signup
{
    
    public class SignupRequestValidator : AbstractValidator<SignupRequest>
    { 
        public SignupRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name is required.");
            
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

            RuleFor(x => x.TaxId)
                .NotEmpty()
                .WithMessage("Tax ID is required.")
                .Must(x => Helpers.ValidTaxId(x))
                .WithMessage("Must be a valid tax ID.");
            
            RuleFor(x => x.WalletAddress)
                .NotEmpty()
                .WithMessage("Wallet address is required.");

            RuleFor(x => x.Address).NotNull().WithMessage("Address is required.")
                .ChildRules(address =>
                {
                    address.RuleFor(a => a.Street)
                        .NotEmpty()
                        .WithMessage("Street is required.");
                    
                    address.RuleFor(a => a.City)
                        .NotEmpty()
                        .WithMessage("City is required.")
                        .Must(a => a.All(char.IsLetter) || a.All(c => char.IsLetter(c) || char.IsWhiteSpace(c)))
                        .WithMessage("City is required.");
                    
                    address.RuleFor(a => a.State)
                        .NotEmpty()
                        .WithMessage("State is required.")
                        .Must(a => a.Length == 2 && a.All(char.IsLetter))
                        .WithMessage("State must be a 2-letter code.");
                    
                    address.RuleFor(a => a.ZipCode)
                        .NotEmpty()
                        .WithMessage("Zip code is required.")
                        .Must(a => a.All(char.IsDigit));
                    
                    address.RuleFor(a => a.CountryCode)
                        .NotEmpty()
                        .WithMessage("Country code is required.")
                        .Must(a => a.Length == 2 && a.All(char.IsLetter))
                        .WithMessage("Country code must be a 2-letter ISO code.");
                });
        }
    }
}