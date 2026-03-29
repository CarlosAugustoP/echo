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
                .WithMessage("Name must be at most 100 characters long.")
                .When(x => x.Name != null); 

            RuleFor(x => x.Email)
                .EmailAddress()
                .WithMessage("Invalid email format.")
                .When(x => x.Email != null);

            RuleFor(x => x.ProfilePictureBase64)
                .Must(ApplicationHelper.IsAValidBase64String!)
                .When(x => !string.IsNullOrEmpty(x.ProfilePictureBase64))
                .WithMessage("Profile picture must be a valid Base64 string.");
        }


    }
}