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
                .WithMessage("O nome não pode ser vazio.")
                .MaximumLength(30)
                .WithMessage("O nome deve ter no máximo 30 caracteres.");

            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("A descrição não pode ser vazia.")
                .MaximumLength(200)
                .WithMessage("A descrição deve ter no máximo 200 caracteres.");

        }
    }
}
