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
                .Must(BeValidDate).WithMessage("Start date is not a valid date");

            RuleFor(x => x.EndDate)
                .NotEmpty().WithMessage("End date is required")
                .Must(BeValidDate).WithMessage("End date is not a valid date");

            RuleFor(x => x.PrimaryMatchday)
                .NotEmpty().WithMessage("Primary matchday is requird")
                .Must(BeValidDay).WithMessage("Primary matchday is not a valid day");

            RuleFor(x => x.AlternativeMatchday)
                .NotEmpty().WithMessage("Alternative matchday is requird")
                .Must(BeValidDay).WithMessage("Alternative matchday is not a valid day");
        }

        private bool BeValidDate(string date)
        {
            return DateTime.TryParseExact(date, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _);
        }

        private bool BeValidDay(string day)
        {
            //Returns true if the supplied string matches any of the values below
            //Otherwise returns false
            string[] validDays = { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
            return validDays.Contains(day);
        }
    }
}