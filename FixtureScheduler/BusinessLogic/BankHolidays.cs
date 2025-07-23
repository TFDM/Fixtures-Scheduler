using System.Text.Json;

namespace BusinessLogic
{
    public class BankHolidays : Interfaces.IBankHolidays
    {
        public List<Models.BankHolidayEvent>? GetBankHolidays(DateTime startDate, DateTime endDate)
        {
            // Specifies the base url of the uk government website - this is where we'll get the bank holidays from
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://www.gov.uk/");

            // Make a get request to get the bank holiday data from the government website
            var response = httpClient.GetAsync("bank-holidays.json").GetAwaiter().GetResult();

            // Check if the response status code was successful
            if (response.IsSuccessStatusCode)
            {
                // Read the json response
                var json = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                // Deserialize json into the BankHolidays object, ignoring case differences in property names
                var bankHolidays = JsonSerializer.Deserialize<Models.BankHolidays>(
                    json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );

                // Return the england and wales bankholiday events which are after the start date
                // and before the end date. Otherwise return null
                return bankHolidays?.EnglandAndWales?.Events.Where(
                    e => e.Date > startDate && e.Date < endDate).ToList()
                    ?? null;
            }

            return null;
        }
    }
}