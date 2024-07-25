using Newtonsoft.Json;

namespace CountryAPI.models.inner
{

    public class NativeName
    {
        [JsonProperty("official")]
        public string? Official { get; set; }

        [JsonProperty("common")]
        public string? Common { get; set; }
    }
}