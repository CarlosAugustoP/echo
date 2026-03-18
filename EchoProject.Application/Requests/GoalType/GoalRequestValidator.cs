using EchoProject.Domain.ProjectAggregate;
using FluentValidation;

namespace EchoProject.Application.Requests.GoalType
{
    public class GoalRequestValidator : AbstractValidator<GoalTypeRequest>
    {
        public GoalRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name cannot be null")
                .MaximumLength(30)
                .WithMessage("Name must be at most 30 characters");

            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("Description cannot be null")
                .MaximumLength(200)
                .WithMessage("Description must be at most 200 characters");

        }
    }
}