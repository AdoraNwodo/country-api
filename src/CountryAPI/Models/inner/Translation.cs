using Newtonsoft.Json;

namespace CountryAPI.models.inner
{
    public class Translation
    {
        [JsonProperty("official")]
        public string? Official { get; set; }

        [JsonProperty("common")]
        public string? Common { get; set; }
    }
}