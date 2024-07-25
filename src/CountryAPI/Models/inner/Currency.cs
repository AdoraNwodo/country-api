using Newtonsoft.Json;

namespace CountryAPI.models.inner
{

    public class Currency
    {
        [JsonProperty("name")]
        public string? Name { get; set; }

        [JsonProperty("symbol")]
        public string? Symbol { get; set; }
    }
}