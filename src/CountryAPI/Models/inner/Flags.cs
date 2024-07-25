using Newtonsoft.Json;

namespace CountryAPI.models.inner
{
    public class Flags
    {
        [JsonProperty("png")]
        public string? Png { get; set; }

        [JsonProperty("svg")]
        public string? Svg { get; set; }

        [JsonProperty("alt")]
        public string? Alt { get; set; }
    }
}