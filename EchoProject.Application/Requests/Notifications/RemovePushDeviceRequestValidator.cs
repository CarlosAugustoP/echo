using FluentValidation;

namespace EchoProject.Application.Requests.Notifications
{
    public class RemovePushDeviceRequestValidator : AbstractValidator<RemovePushDeviceRequest>
    {
        public RemovePushDeviceRequestValidator()
        {
            RuleFor(x => x.Token)
                .NotEmpty()
                .MaximumLength(512);
        }
    }
}
