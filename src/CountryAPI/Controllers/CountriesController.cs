using Microsoft.AspNetCore.Mvc;

namespace CountryAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CountriesController : ControllerBase
    {
        private readonly IRestCountriesService _countriesService;
        private readonly ILogger<CountriesController> _logger;

        public CountriesController(IRestCountriesService countriesService, ILogger<CountriesController> logger)
        {
            _countriesService = countriesService;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves a paginated list of countries with optional sorting and filtering.
        /// </summary>
        /// <param name="page">The page number to retrieve. Defaults to 1. Must be greater than 0.</param>
        /// <param name="pageSize">The number of countries per page. Defaults to 10. Must be greater than 0 and less than or equal to 100.</param>
        /// <param name="sortBy">The field by which to sort the countries (e.g., "common" for common name, "official" for official name). Defaults to "common".</param>
        /// <param name="sorted">Indicates if the results should be sorted. Defaults to true.</param>
        /// <param name="search">Search query to filter countries by name. Searches in common name. Length should not exceed 100 characters.</param>
        /// <returns>A paginated list of countries.</returns>
        /// <response code="200">Returns the list of countries</response>
        /// <response code="400">If input parameters are invalid</response>
        /// <response code="500">If an error occurs while processing the request</response>
        [HttpGet]
        public async Task<ActionResult<PaginatedResponse<Country>>> GetCountries(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? sortBy = "common",
            [FromQuery] bool sorted = true,
            [FromQuery] string? search = null)
        {
            if (!IsValidPaginationParameters(page, pageSize) || !IsValidSearchQuery(search))
            {
                return BadRequest(new { Message = "Invalid request parameters." });
            }

            try
            {
                var countries = await _countriesService.GetAllCountriesAsync(sortBy, sorted, search);

                var totalItems = countries.Count;
                var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
                var hasNext = page < totalPages;

                var pagedCountries = countries
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var response = new PaginatedResponse<Country>
                {
                    CurrentPage = page,
                    PageSize = pageSize,
                    TotalItems = totalItems,
                    TotalPages = totalPages,
                    HasNext = hasNext,
                    Data = pagedCountries
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving countries.");
                return StatusCode(500, new { Message = $"An error occurred while processing your request. Message: {ex.Message}" });
            }
        }

        /// <summary>
        /// Retrieves information about a specific country by its code.
        /// </summary>
        /// <param name="code">The country code (e.g., "US" for the United States, "CA" for Canada). Must be a valid ISO 3166-1 alpha-2, alpha-3, or numeric code.</param>
        /// <returns>The country details if found.</returns>
        /// <response code="200">Returns the country details</response>
        /// <response code="400">If the country code is invalid</response>
        /// <response code="404">If the country is not found</response>
        /// <response code="500">If an error occurs while processing the request</response>
        [HttpGet("{code}")]
        public async Task<IActionResult> GetCountryByCode(string code)
        {
            if (!IsValidCountryCode(code))
            {
                return BadRequest(new { Message = "Invalid country code." });
            }

            try
            {
                var country = await _countriesService.GetCountryByCodeAsync(code);
                if (country == null)
                {
                    return NotFound(new { Message = "Country code does not exist." });
                }

                return Ok(country);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving country by code.");
                return StatusCode(500, new { Message = $"An error occurred while processing your request. Message: {ex.Message}" });
            }
        }

        /// <summary>
        /// Retrieves a list of regions and the countries within each region.
        /// </summary>
        /// <param name="hasCountryDetails">Indicates if detailed country information should be included. Defaults to true.</param>
        /// <param name="search">Search query to filter regions by name or countries by name within each region. Length should not exceed 100 characters.</param>
        /// <returns>A list of regions with countries.</returns>
        /// <response code="200">Returns the list of regions with countries</response>
        /// <response code="400">If the search query is invalid</response>
        /// <response code="404">If no regions are found</response>
        /// <response code="500">If an error occurs while processing the request</response>
        [HttpGet("/api/regions")]
        public async Task<IActionResult> GetRegions([FromQuery] bool hasCountryDetails = true, [FromQuery] string? search = null)
        {
            if (!IsValidSearchQuery(search))
            {
                return BadRequest(new { Message = "Invalid search query." });
            }

            try
            {
                var regions = await _countriesService.GetRegionsAsync(hasCountryDetails, search);
                if (regions == null || regions?.Count == 0)
                {
                    return NotFound(new { Message = "No regions found." });
                }

                return Ok(regions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving regions.");
                return StatusCode(500, new { Message = $"An error occurred while processing your request. Message: {ex.Message}" });
            }
        }

        /// <summary>
        /// Retrieves a list of languages spoken and the countries where they are spoken.
        /// </summary>
        /// <param name="hasCountryDetails">Indicates if detailed country information should be included. Defaults to true.</param>
        /// <param name="search">Search query to filter languages by name or countries by name where the language is spoken. Length should not exceed 100 characters.</param>
        /// <returns>A list of languages with countries.</returns>
        /// <response code="200">Returns the list of languages with countries</response>
        /// <response code="400">If the search query is invalid</response>
        /// <response code="404">If no languages are found</response>
        /// <response code="500">If an error occurs while processing the request</response>
        [HttpGet("/api/languages")]
        public async Task<IActionResult> GetLanguages([FromQuery] bool hasCountryDetails = true, [FromQuery] string? search = null)
        {
            if (!IsValidSearchQuery(search))
            {
                return BadRequest(new { Message = "Invalid search query." });
            }

            try
            {
                var languages = await _countriesService.GetLanguagesAsync(hasCountryDetails, search);
                if (languages == null || languages.Count == 0)
                {
                    return NotFound(new { Message = "No languages found." });
                }

                return Ok(languages);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving languages.");
                return StatusCode(500, new { Message = $"An error occurred while processing your request. Message: {ex.Message}" });
            }
        }

        /* Input validation methods */

        private bool IsValidPaginationParameters(int page, int pageSize)
        {
            return page > 0 && pageSize > 0 && pageSize <= 100;
        }

        private bool IsValidSearchQuery(string? search)
        {
            return string.IsNullOrEmpty(search) || search.Length <= 100;
        }

        private bool IsValidCountryCode(string code)
        {
            return !string.IsNullOrEmpty(code) && code.Length <= 4;
        }
    }
}
