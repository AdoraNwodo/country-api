using Newtonsoft.Json;

namespace CountryAPI.models.inner
{

    public class Idd
    {
        [JsonProperty("root")]
        public string? Root { get; set; }

        [JsonProperty("suffixes")]
        public List<string>? Suffixes { get; set; }
    }
}