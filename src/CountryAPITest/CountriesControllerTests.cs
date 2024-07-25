using CountryAPI.Controllers;
using CountryAPI.models.inner;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

public class CountriesControllerTests
{
    private readonly Mock<IRestCountriesService> _countriesServiceMock;
    private readonly Mock<ILogger<CountriesController>> _loggerMock;

    public CountriesControllerTests()
    {
        _countriesServiceMock = new Mock<IRestCountriesService>();
        _loggerMock = new Mock<ILogger<CountriesController>>();
    }

    [Fact]
    public async Task GetCountries_ReturnsOkResult_WithListOfCountries()
    {
        // Arrange
        var countries = new List<Country>
        {
            new Country { Name = new Name { Common = "Country1" } }
        };

        var paginatedResponse = new PaginatedResponse<Country>
        {
            CurrentPage = 1,
            PageSize = 10,
            TotalItems = countries.Count,
            TotalPages = 1,
            HasNext = false,
            Data = countries
        };
        _countriesServiceMock.Setup(s => s.GetAllCountriesAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string>())).ReturnsAsync(countries);

        var controller = new CountriesController(_countriesServiceMock.Object, _loggerMock.Object);

        // Act
        var result = await controller.GetCountries();

        // Assert
        var actionResult = Assert.IsType<ActionResult<PaginatedResponse<Country>>>(result);
        var objectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        Assert.Equal(200, objectResult.StatusCode); // Check for OK status code

        var returnValue = Assert.IsType<PaginatedResponse<Country>>(objectResult.Value);
        Assert.Single(returnValue.Data);
        Assert.Equal("Country1", returnValue.Data.First().Name.Common);
    }



    [Fact]
    public async Task GetCountryByCode_ReturnsOkResult_WithCountry()
    {
        // Arrange
        var country = new Country { Name = new Name { Common = "Country1" }, Cca2 = "C1" };
        _countriesServiceMock.Setup(s => s.GetCountryByCodeAsync(It.IsAny<string>())).ReturnsAsync(country);

        var controller = new CountriesController(_countriesServiceMock.Object, _loggerMock.Object);

        // Act
        var result = await controller.GetCountryByCode("C1");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<Country>(okResult.Value);
        Assert.Equal("Country1", returnValue.Name.Common);
    }

    [Fact]
    public async Task GetRegions_ReturnsOkResult_WithListOfRegions()
    {
        // Arrange
        var regions = new List<RegionResponse>
        {
            new RegionResponse { Name = "Region1", Countries = new List<object> { new Country { Name = new Name { Common = "Country1" } } } }
        };
        _countriesServiceMock.Setup(s => s.GetRegionsAsync(It.IsAny<bool>(), It.IsAny<string>())).ReturnsAsync(regions);

        var controller = new CountriesController(_countriesServiceMock.Object, _loggerMock.Object);

        // Act
        var result = await controller.GetRegions(true);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<List<RegionResponse>>(okResult.Value);
        Assert.Single(returnValue);
        Assert.Equal("Region1", returnValue[0].Name);
    }

    [Fact]
    public async Task GetLanguages_ReturnsOkResult_WithListOfLanguages()
    {
        // Arrange
        var languages = new List<LanguageResponse>
        {
            new LanguageResponse { Language = "en", Countries = new List<object> { new Country { Name = new Name { Common = "Country1" } } } }
        };
        _countriesServiceMock.Setup(s => s.GetLanguagesAsync(It.IsAny<bool>(), It.IsAny<string>())).ReturnsAsync(languages);

        var controller = new CountriesController(_countriesServiceMock.Object, _loggerMock.Object);

        // Act
        var result = await controller.GetLanguages(true);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<List<LanguageResponse>>(okResult.Value);
        Assert.Single(returnValue);
        Assert.Equal("en", returnValue[0].Language);
    }
}
