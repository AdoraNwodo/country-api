using CountryAPI.models.inner;
using Newtonsoft.Json;

namespace CountryAPI
{

    public class Country
    {
        [JsonProperty("name")]
        public Name? Name { get; set; }

        [JsonProperty("tld")]
        public List<string>? Tld { get; set; }

        [JsonProperty("cca2")]
        public string? Cca2 { get; set; }

        [JsonProperty("ccn3")]
        public string? Ccn3 { get; set; }

        [JsonProperty("cca3")]
        public string? Cca3 { get; set; }

        [JsonProperty("cioc")]
        public string? Cioc { get; set; }

        [JsonProperty("independent")]
        public bool? Independent { get; set; }

        [JsonProperty("status")]
        public string? Status { get; set; }

        [JsonProperty("unMember")]
        public bool? UnMember { get; set; }

        [JsonProperty("currencies")]
        public Dictionary<string, Currency>? Currencies { get; set; }

        [JsonProperty("idd")]
        public Idd? Idd { get; set; }

        [JsonProperty("capital")]
        public List<string>? Capital { get; set; }

        [JsonProperty("altSpellings")]
        public List<string>? AltSpellings { get; set; }

        [JsonProperty("region")]
        public string? Region { get; set; }

        [JsonProperty("languages")]
        public Dictionary<string, string>? Languages { get; set; }

        [JsonProperty("translations")]
        public Dictionary<string, Translation>? Translations { get; set; }

        [JsonProperty("latlng")]
        public List<double>? Latlng { get; set; }

        [JsonProperty("landlocked")]
        public bool Landlocked { get; set; }

        [JsonProperty("area")]
        public double Area { get; set; }

        [JsonProperty("demonyms")]
        public Demonyms? Demonyms { get; set; }

        [JsonProperty("flag")]
        public string? Flag { get; set; }

        [JsonProperty("maps")]
        public Maps? Maps { get; set; }

        [JsonProperty("population")]
        public int Population { get; set; }

        [JsonProperty("car")]
        public Car? Car { get; set; }

        [JsonProperty("timezones")]
        public List<string>? Timezones { get; set; }

        [JsonProperty("continents")]
        public List<string>? Continents { get; set; }

        [JsonProperty("flags")]
        public Flags? Flags { get; set; }

        [JsonProperty("coatOfArms")]
        public CoatOfArms? CoatOfArms { get; set; }

        [JsonProperty("startOfWeek")]
        public string? StartOfWeek { get; set; }

        [JsonProperty("capitalInfo")]
        public CapitalInfo? CapitalInfo { get; set; }
    }
}