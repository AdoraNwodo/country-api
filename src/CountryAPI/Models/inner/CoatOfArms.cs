using Newtonsoft.Json;

namespace CountryAPI.models.inner
{
    public class CoatOfArms
    {
        [JsonProperty("png")]
        public string? Png { get; set; }

        [JsonProperty("svg")]
        public string? Svg { get; set; }
    }
}