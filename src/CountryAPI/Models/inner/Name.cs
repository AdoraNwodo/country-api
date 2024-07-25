using Newtonsoft.Json;

namespace CountryAPI.models.inner
{

    public class Name
    {
        [JsonProperty("common")]
        public string? Common { get; set; }

        [JsonProperty("official")]
        public string? Official { get; set; }

        [JsonProperty("nativeName")]
        public Dictionary<string, NativeName>? NativeName { get; set; }
    }
}