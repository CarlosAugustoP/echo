using EchoProject.Application.Common.Utils;
using FluentValidation;

namespace EchoProject.Application.Requests.Users
{
    public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
    {
        public UpdateUserRequestValidator()
        {
            RuleFor(x => x.Name)
                .MaximumLength(100)
                .WithMessage("O nome deve ter no máximo 100 caracteres.")
                .When(x => x.Name != null);

            RuleFor(x => x.Email)
                .EmailAddress()
                .WithMessage("Formato de e-mail inválido.")
                .When(x => x.Email != null);

            RuleFor(x => x.Bio)
                .MaximumLength(1000)
                .WithMessage("A bio deve ter no máximo 1000 caracteres.")
                .When(x => x.Bio != null);

            RuleFor(x => x.ProfilePictureBase64)
                .Must(ApplicationHelper.IsAValidBase64String!)
                .When(x => !string.IsNullOrEmpty(x.ProfilePictureBase64))
                .WithMessage("A foto de perfil deve ser uma string Base64 válida.");
        }
    }
}
