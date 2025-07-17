using FluentValidation;

namespace Validation
{
    public class ExcludedDatesValidation : AbstractValidator<DTOs.ExcludedDatesDTO>
    {
        public ExcludedDatesValidation()
        {
            RuleFor(x => x.Date)
                .NotEmpty().WithMessage("Date is required")
                .Must(ValidationHelpers.BeValidDate).WithMessage("Date is not valid");

            RuleFor(x => x.Reason)
                .NotEmpty().WithMessage("Reason is required");
        }
    }
}