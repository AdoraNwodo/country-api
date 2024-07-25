using Newtonsoft.Json;

namespace CountryAPI.models.inner
{
    public class Car
    {
        [JsonProperty("signs")]
        public List<string>? Signs { get; set; }

        [JsonProperty("side")]
        public string? Side { get; set; }
    }

}