using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace CountryAPI
{
    public class RestCountriesService : IRestCountriesService
    {
        private const string BASE_URL = "https://restcountries.com/v3.1";
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<RestCountriesService> _logger;
        private readonly CacheSettings _cacheSettings;
        private static readonly SemaphoreSlim CacheSemaphore = new SemaphoreSlim(1, 1);

        public RestCountriesService(HttpClient httpClient, IMemoryCache memoryCache, ILogger<RestCountriesService> logger, IOptions<CacheSettings> cacheSettings)
        {
            _httpClient = httpClient;
            _memoryCache = memoryCache;
            _logger = logger;
            _cacheSettings = cacheSettings.Value;
        }

        public async Task<IList<Country>> GetAllCountriesAsync(string? sortBy, bool sorted, string? search = null)
        {
            try
            {
                var (countries, fetchedFromApi) = await FetchCountriesFromCacheOrApiAsync();

                if (fetchedFromApi)
                {
                    await UpdateCacheAsync(countries);
                }

                if (!string.IsNullOrEmpty(search))
                {
                    countries = countries.Where(c => c.Name!.Common!.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
                }

                if (sorted)
                {
                    countries = SortCountries(countries, sortBy);
                }

                return countries;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving countries.");
                throw;
            }
        }

        public async Task<Country?> GetCountryByCodeAsync(string code)
        {
            try
            {
                Country? country;
                var cacheHasData = _memoryCache.TryGetValue(_cacheSettings.CacheKey, out IList<Country> countries);

                if (!cacheHasData || countries == null || countries.Count == 0)
                {
                    country = await FetchCountryByCodeAsync(code);
                }
                else
                {
                    country = countries?.FirstOrDefault(c =>
                        string.Equals(c.Cca2, code, StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(c.Ccn3, code, StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(c.Cca3, code, StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(c.Cioc, code, StringComparison.OrdinalIgnoreCase));
                }

                return country;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving country by code.");
                throw;
            }
        }

        public async Task<IList<RegionResponse>?> GetRegionsAsync(bool hasCountryDetails, string? search = null)
        {
            try
            {
                var (countries, _) = await FetchCountriesFromCacheOrApiAsync();

                var regions = countries?
                    .Where(c => !string.IsNullOrEmpty(c.Region))
                    .GroupBy(c => c.Region)
                    .Select(group => new RegionResponse
                    {
                        Name = group.Key,
                        Countries = hasCountryDetails
                            ? group.Cast<object>().ToList()
                            : group.Select(c => new CountrySummary
                            {
                                Common = c.Name?.Common,
                                Official = c.Name?.Official,
                                Capital = c.Capital?.FirstOrDefault()
                            }).Cast<object>().ToList()
                    })
                    .ToList();

                if (!string.IsNullOrEmpty(search))
                {
                    regions = regions?
                        .Where(r => r.Name!.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                                    r.Countries!.Any(c => c is CountrySummary cs && cs.Common!.Contains(search, StringComparison.OrdinalIgnoreCase)))
                        .ToList();
                }

                return regions;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving regions.");
                throw;
            }
        }

        public async Task<IList<LanguageResponse>?> GetLanguagesAsync(bool hasCountryDetails, string? search = null)
        {
            try
            {
                var (countries, _) = await FetchCountriesFromCacheOrApiAsync();

                var languages = countries?
                        .Where(c => c.Languages != null && c.Languages.Any())
                        .SelectMany(c => c.Languages!.Select(lang => new { lang.Key, Country = c }))
                        .GroupBy(x => x.Key)
                        .Select(group => new LanguageResponse
                        {
                            Language = group.Key,
                            Countries = hasCountryDetails
                                ? group.Select(x => x.Country).Cast<object>().ToList()
                                : group.Select(x => new CountrySummary
                                {
                                    Common = x.Country.Name?.Common,
                                    Official = x.Country.Name?.Official,
                                    Capital = x.Country.Capital?.FirstOrDefault()
                                }).Cast<object>().ToList()
                        })
                        .ToList();

                if (!string.IsNullOrEmpty(search))
                {
                    languages = languages?
                        .Where(l => l.Language!.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                                    l.Countries!.Any(c => c is CountrySummary cs && cs.Common!.Contains(search, StringComparison.OrdinalIgnoreCase)))
                        .ToList();
                }

                return languages;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving languages.");
                throw;
            }
        }

        private async Task<(IList<Country>, bool)> FetchCountriesFromCacheOrApiAsync()
        {
            var cacheHasData = _memoryCache.TryGetValue(_cacheSettings.CacheKey, out IList<Country> countries);
            var fetchedFromApi = false;

            if (!cacheHasData || countries == null || countries.Count == 0)
            {
                var response = await _httpClient.GetAsync($"{BASE_URL}/all");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    countries = JsonConvert.DeserializeObject<IList<Country>>(content)!;
                    fetchedFromApi = true;
                }
                else
                {
                    _logger.LogError($"Failed to retrieve countries data. Status code: {response.StatusCode}");
                    throw new HttpRequestException($"Failed to retrieve countries data. Status code: {response.StatusCode}");
                }
            }

            return (countries ?? new List<Country>(), fetchedFromApi);
        }

        private IList<Country> SortCountries(IList<Country> countries, string? sortBy)
        {
            return sortBy?.ToLower() switch
            {
                "common" => countries.OrderBy(c => c.Name?.Common).ToList(),
                "official" => countries.OrderBy(c => c.Name?.Official).ToList(),
                _ => countries.OrderBy(c => c.Name?.Common).ToList(),
            };
        }

        private async Task<Country?> FetchCountryByCodeAsync(string code)
        {
            var response = await _httpClient.GetAsync($"{BASE_URL}/alpha/{code}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var countries = JsonConvert.DeserializeObject<IList<Country>>(content);
                return countries?.FirstOrDefault();
            }
            else
            {
                _logger.LogError($"Failed to retrieve country data for code {code}. Status code: {response.StatusCode}");
                throw new HttpRequestException($"Failed to retrieve country data for code {code}. Status code: {response.StatusCode}");
            }
        }

        private async Task UpdateCacheAsync(IList<Country> countries)
        {
            await CacheSemaphore.WaitAsync(); // Only one process should attempt to update the cache at any time
            try
            {
                _memoryCache.Set(_cacheSettings.CacheKey, countries, TimeSpan.FromHours(_cacheSettings.CacheDurationInHours));
            }
            finally
            {
                CacheSemaphore.Release();
            }
        }
    }
}
