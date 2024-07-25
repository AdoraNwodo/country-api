using Newtonsoft.Json;

namespace CountryAPI.models.inner
{
    public class CapitalInfo
    {
        [JsonProperty("latlng")]
        public List<double>? Latlng { get; set; }
    }

}