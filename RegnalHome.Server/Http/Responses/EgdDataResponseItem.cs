using Newtonsoft.Json;

namespace RegnalHome.Server.Http.Responses
{
    public class EgdDataResponseItem
    {
        [JsonProperty("ean/eic")]
        public string eanEic { get; set; }

        public string profile { get; set; }

        public string units { get; set; }

        public int total { get; set; }

        public IEnumerable<EgdDataResponseItemData> data { get; set; }
    }
}
