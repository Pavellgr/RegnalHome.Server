using System.Globalization;

namespace RegnalHome.Web.Services
{
    public class TranslationService : ITranslationService
    {
        private const int UniversalLCID = 1033;

        public string Get(string label, int? cultureLCID = null)
        {
            return (keyValuePairs.GetValueOrDefault(cultureLCID ?? CultureInfo.CurrentCulture.LCID) ?? keyValuePairs.GetValueOrDefault(UniversalLCID))
                .GetValueOrDefault(label)
                ?? label;
        }

        private Dictionary<int, Dictionary<string, string>> keyValuePairs = new Dictionary<int, Dictionary<string, string>>
        {
            {
                UniversalLCID, //English - United States
                new Dictionary<string, string>
                {
                    { "Name", "Name" }
                }
            },
            {
                1029, //Czech
                new Dictionary<string, string>
                {
                    { "Name", "Jméno" },
                    { "LastCommunication", "Poslední komunikace" },
                    { "IrrigationLengthMinutes", "Délka zalévání v minutách" },
                    { "IrrigationTime", "Čas zalévání" },
                    { "Submit", "Uložit" }
                }
            }
        };
    }
}
