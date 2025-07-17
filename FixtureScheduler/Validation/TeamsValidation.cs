using FluentValidation;

namespace Validation
{
    public class TeamsValidation : AbstractValidator<DTOs.TeamsDTO>
    {
        public TeamsValidation()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Team name is required");
        }
    }
}