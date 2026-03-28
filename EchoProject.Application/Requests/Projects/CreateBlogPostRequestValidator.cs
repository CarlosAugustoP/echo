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

            RuleFor(x => x.ProjectId)
                .NotEqual(Guid.Empty).WithMessage("ProjectId cannot be empty.");

            RuleFor(x => x.HeaderImageBase64)
                .Must(x => x == null || (x.StartsWith("data:image") && x.Length > 0))
                .WithMessage("HeaderImageBase64 must be a valid base64 image string if provided.");

            RuleFor(x => x.ImageBase64List)
                .Must(x => x == null || (x.Count > 0 && x.All(img => img.StartsWith("data:image"))))
                .WithMessage("ImageBase64List must contain valid base64 image strings if provided.");
        }
    }
}