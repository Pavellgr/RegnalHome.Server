using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RegnalHome.Server.Http.Responses;

namespace RegnalHome.Server.Http.JsonConverters
{
    public class EgdDataResponseConverter : JsonConverter<EgdDataResponse>
    {
        public override EgdDataResponse ReadJson(JsonReader reader, Type objectType, EgdDataResponse existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            // Read the JSON array into a JArray object
            JArray jsonArray = JArray.Load(reader);

            // Create a new instance of EgdDataResponse
            EgdDataResponse egdDataResponse = new EgdDataResponse();

            // Deserialize the JSON array into the data property of EgdDataResponse
            egdDataResponse.Items = jsonArray.ToObject<List<EgdDataResponseItem>>();

            return egdDataResponse;
        }

        public override void WriteJson(JsonWriter writer, EgdDataResponse value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }

}
