namespace Models
{
    public class MenuItem
    {
        public int Option { get; set; }
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// This is required to allow spectre console to show a multi 
        /// select prompt of bank holidays
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Description}";
        }
    }
}