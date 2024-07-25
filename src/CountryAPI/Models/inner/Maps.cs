using Newtonsoft.Json;

namespace CountryAPI.models.inner
{
    public class Maps
    {
        [JsonProperty("googleMaps")]
        public string? GoogleMaps { get; set; }

        [JsonProperty("openStreetMaps")]
        public string? OpenStreetMaps { get; set; }
    }

}