using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace RegnalHome.Server.Http.Responses
{
    public class EgdDataResponse : IHttpResponse
    {
        public ICollection<EgdDataResponseItem> Items { get; set; }
    }
}
