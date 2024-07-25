using Newtonsoft.Json;

namespace CountryAPI.models.inner
{
    public class Demonyms
    {
        [JsonProperty("eng")]
        public Demonym? Eng { get; set; }
    }
}