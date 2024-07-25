namespace CountryAPI
{
    public interface IRestCountriesService
    {
        Task<IList<Country>> GetAllCountriesAsync(string? sortBy, bool sorted, string? search);
        Task<Country?> GetCountryByCodeAsync(string code);
        Task<IList<RegionResponse>?> GetRegionsAsync(bool hasCountryDetails, string? search);
        Task<IList<LanguageResponse>?> GetLanguagesAsync(bool hasCountryDetails, string? search);
    }
}