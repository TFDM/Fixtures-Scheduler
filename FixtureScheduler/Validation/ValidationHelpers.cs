using System.Globalization;

namespace Validation
{
    public class ValidationHelpers()
    {
        /// <summary>
        /// Ensures a string is a valid date in uk format
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static bool BeValidDate(string date)
        {
            return DateTime.TryParseExact(date, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _);
        }

        /// <summary>
        /// Ensures a string is a valid day of the week
        /// </summary>
        /// <param name="day"></param>
        /// <returns></returns>
        public static bool BeValidDay(string day)
        {
            //Returns true if the supplied string matches any of the values below
            //Otherwise returns false
            string[] validDays = { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
            return validDays.Contains(day);
        }
    }
}