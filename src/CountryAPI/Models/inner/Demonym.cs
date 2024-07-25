using Newtonsoft.Json;

namespace CountryAPI.models.inner
{
    public class Demonym
    {
        [JsonProperty("f")]
        public string? Female { get; set; }

        [JsonProperty("m")]
        public string? Male { get; set; }
    }
}