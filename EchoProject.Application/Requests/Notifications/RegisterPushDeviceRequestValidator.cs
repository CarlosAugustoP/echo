using FluentValidation;

namespace EchoProject.Application.Requests.Notifications
{
    public class RegisterPushDeviceRequestValidator : AbstractValidator<RegisterPushDeviceRequest>
    {
        public RegisterPushDeviceRequestValidator()
        {
            RuleFor(x => x.Token)
                .NotEmpty()
                .MaximumLength(512);

            RuleFor(x => x.Platform)
                .NotEmpty()
                .MaximumLength(50);
        }
    }
}
