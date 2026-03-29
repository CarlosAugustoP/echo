using EchoProject.Application.Common.Utils;
using FluentValidation;

namespace EchoProject.Application.Requests.Projects
{
    public class CreateBlogPostRequestValidator : AbstractValidator<CreateBlogPostRequest>
    {
        public CreateBlogPostRequestValidator()
        {
            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Content is required.")
                .MinimumLength(10).WithMessage("Content must be at least 10 characters long.");

            RuleFor(x => x.HeaderImageBase64)
                .Must(x => x is null || ApplicationHelper.IsAValidBase64String(x))
                .WithMessage("HeaderImageBase64 must be a valid base64 image string if provided.");
        }
    
    }
}