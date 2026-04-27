using EchoProject.Application.Common.Utils;
using FluentValidation;

namespace EchoProject.Application.Requests.Projects
{
    public class CreateBlogPostRequestValidator : AbstractValidator<CreateBlogPostRequest>
    {
        public CreateBlogPostRequestValidator()
        {
            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("O conteúdo é obrigatório.")
                .MinimumLength(10).WithMessage("O conteúdo deve ter pelo menos 10 caracteres.");

            RuleFor(x => x.HeaderImageBase64)
                .Must(x => x is null || ApplicationHelper.IsAValidBase64String(x))
                .WithMessage("HeaderImageBase64 deve ser uma string Base64 de imagem válida, se informada.");
        }
    
    }
}
