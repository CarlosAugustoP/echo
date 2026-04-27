using EchoProject.Application.Common.Utils;
using FluentValidation;

namespace EchoProject.Application.Requests.Docs
{
    public class DocumentRequestValidator : AbstractValidator<DocumentRequest>
    {
        public DocumentRequestValidator()
        {
            RuleFor(x => x.Base64String)
                .NotEmpty()
                .WithMessage("A string Base64 não pode estar vazia.")
                .Must(ApplicationHelper.IsAValidBase64String!)
                .WithMessage("A string informada não é um Base64 válido.");
        }
    }
}
