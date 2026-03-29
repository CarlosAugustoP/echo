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
                .WithMessage("Base64 string cannot be empty.")
                .Must(ApplicationHelper.IsAValidBase64String!)
                .WithMessage("The provided string is not a valid Base64 encoded string.");
        }
    }
}