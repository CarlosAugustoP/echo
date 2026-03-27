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
                .Must(BeAValidBase64String!)
                .When(x => !string.IsNullOrEmpty(x.ProfilePictureBase64))
                .WithMessage("Profile picture must be a valid Base64 string.");
        }

        private bool BeAValidBase64String(string base64String)
        {
            if (string.IsNullOrEmpty(base64String)) return true;

            var base64Data = base64String.Contains(",") ? base64String.Split(',')[1] : base64String;

            try
            {
                Convert.FromBase64String(base64Data);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}