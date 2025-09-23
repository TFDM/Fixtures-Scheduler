namespace Models
{
    public class AvailableDates
    {
        public DateTime Date { get; set; }
        public bool IsPrimaryMatchday { get; set; }
        public bool IsBankHoliday { get; set; }
        public string? Description { get; set; }

        /// <summary>
        /// This is required to allow spectre console to show a multi 
        /// select prompt of bank holidays
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Date:dd/MM/yyyy}";
        }
    }
}