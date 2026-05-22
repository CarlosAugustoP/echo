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
                .WithMessage("O nome deve ter no mÃ¡ximo 100 caracteres.")
                .When(x => x.Name != null);

            RuleFor(x => x.Email)
                .EmailAddress()
                .WithMessage("Formato de e-mail invÃ¡lido.")
                .When(x => x.Email != null);

            RuleFor(x => x.Bio)
                .MaximumLength(1000)
                .WithMessage("A bio deve ter no mÃ¡ximo 1000 caracteres.")
                .When(x => x.Bio != null);

            RuleFor(x => x.ProfilePictureBase64)
                .Must(ApplicationHelper.IsAValidBase64String!)
                .When(x => !string.IsNullOrEmpty(x.ProfilePictureBase64))
                .WithMessage("A foto de perfil deve ser uma string Base64 vÃ¡lida.");
        }
    }
}
