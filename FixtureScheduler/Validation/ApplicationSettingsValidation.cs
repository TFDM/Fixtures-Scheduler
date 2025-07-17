using System.Data;
using System.Globalization;
using FluentValidation;

namespace Validation
{
    public class ApplicationSettingsValidation : AbstractValidator<DTOs.ApplicationSettingsDTO>
    {
        public ApplicationSettingsValidation()
        {
            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("Start date is required")
                .Must(ValidationHelpers.BeValidDate).WithMessage("Start date is not a valid date");

            RuleFor(x => x.EndDate)
                .NotEmpty().WithMessage("End date is required")
                .Must(ValidationHelpers.BeValidDate).WithMessage("End date is not a valid date");

            RuleFor(x => x.PrimaryMatchday)
                .NotEmpty().WithMessage("Primary matchday is requird")
                .Must(ValidationHelpers.BeValidDay).WithMessage("Primary matchday is not a valid day");

            RuleFor(x => x.AlternativeMatchday)
                .NotEmpty().WithMessage("Alternative matchday is requird")
                .Must(ValidationHelpers.BeValidDay).WithMessage("Alternative matchday is not a valid day");

            RuleForEach(x => x.ExcludedDates).SetValidator(new ExcludedDatesValidation());
        }
    }
}