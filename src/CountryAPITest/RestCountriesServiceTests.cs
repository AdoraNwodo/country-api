using CountryAPI.models.inner;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using System.Net;

public class RestCountriesServiceTests
{
    private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
    private readonly Mock<IMemoryCache> _memoryCacheMock;
    private readonly Mock<ILogger<RestCountriesService>> _loggerMock;
    private readonly IOptions<CacheSettings> _cacheSettings;
    private readonly RestCountriesService _service;
    private readonly HttpClient _httpClient;
    private readonly Mock<HttpMessageHandler> _handlerMock;

    public RestCountriesServiceTests()
    {
        _httpClientFactoryMock = new Mock<IHttpClientFactory>();
        _memoryCacheMock = new Mock<IMemoryCache>();
        _loggerMock = new Mock<ILogger<RestCountriesService>>();
        _cacheSettings = Options.Create(new CacheSettings { CacheKey = "Countries", CacheDurationInHours = 24 });

        // Setup HttpClient and handler
        _handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        _httpClient = new HttpClient(_handlerMock.Object);
        _httpClientFactoryMock.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(_httpClient);

        // Correctly setup the memory cache mock to handle Set and TryGetValue
        object cacheValue;
        _memoryCacheMock.Setup(mc => mc.TryGetValue(It.IsAny<object>(), out cacheValue)).Returns(false);
        _memoryCacheMock.Setup(mc => mc.CreateEntry(It.IsAny<object>())).Returns(Mock.Of<ICacheEntry>());

        _service = new RestCountriesService(_httpClient, _memoryCacheMock.Object, _loggerMock.Object, _cacheSettings);
    }

    [Fact]
    public async Task GetAllCountriesAsync_ReturnsCountriesList()
    {
        // Arrange
        var countries = new List<Country> { new Country { Name = new Name { Common = "Country1" } } };
        var jsonCountries = JsonConvert.SerializeObject(countries);
        _handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonCountries)
            });

        // Act
        var result = await _service.GetAllCountriesAsync(null, false);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Country1", result[0].Name.Common);
    }

    [Fact]
    public async Task GetAllCountriesAsync_SortsCountries()
    {
        // Arrange
        var countries = new List<Country>
    {
        new Country { Name = new Name { Common = "B Country" } },
        new Country { Name = new Name { Common = "A Country" } },
        new Country { Name = new Name { Common = "C Country" } }
    };
        var jsonCountries = JsonConvert.SerializeObject(countries);

        _handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonCountries)
            });

        // Act
        var result = await _service.GetAllCountriesAsync("common", true);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.Equal("A Country", result[0].Name.Common); // Ensure sorting by "common" name
        Assert.Equal("B Country", result[1].Name.Common);
        Assert.Equal("C Country", result[2].Name.Common);
    }

    [Fact]
    public async Task GetCountryByCodeAsync_ReturnsCountry()
    {
        // Arrange
        var country = new Country
        {
            Name = new Name { Common = "Country1" },
            Cca2 = "C1",
            Ccn3 = "001",
            Cca3 = "C01",
            Cioc = "CIO"
        };
        var jsonCountry = JsonConvert.SerializeObject(new List<Country> { country });

        _handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get && req.RequestUri.ToString().Contains("/alpha/C1")),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonCountry)
            });

        // Act
        var result = await _service.GetCountryByCodeAsync("C1");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Country1", result.Name.Common);
        Assert.Equal("C1", result.Cca2);
        Assert.Equal("001", result.Ccn3);
        Assert.Equal("C01", result.Cca3);
        Assert.Equal("CIO", result.Cioc);
    }

    [Fact]
    public async Task GetRegionsAsync_ReturnsRegionsList()
    {
        // Arrange
        var countries = new List<Country>
    {
        new Country { Name = new Name { Common = "Country1" }, Region = "Region1" },
        new Country { Name = new Name { Common = "Country2" }, Region = "Region1" },
        new Country { Name = new Name { Common = "Country3" }, Region = "Region2" }
    };
        var jsonCountries = JsonConvert.SerializeObject(countries);

        _handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonCountries)
            });

        // Act
        var result = await _service.GetRegionsAsync(true);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count); // Expecting two regions: "Region1" and "Region2"
        Assert.Equal("Region1", result[0].Name);
        Assert.Equal("Region2", result[1].Name);
        Assert.Equal(2, result[0].Countries.Count); // Two countries in Region1
        Assert.Equal(1, result[1].Countries.Count); // One country in Region2
    }

    [Fact]
    public async Task GetLanguagesAsync_ReturnsLanguagesList()
    {
        // Arrange
        var countries = new List<Country>
    {
        new Country
        {
            Name = new Name { Common = "Country1" },
            Languages = new Dictionary<string, string> { { "en", "English" } }
        },
        new Country
        {
            Name = new Name { Common = "Country2" },
            Languages = new Dictionary<string, string> { { "en", "English" }, { "fr", "French" } }
        },
        new Country
        {
            Name = new Name { Common = "Country3" },
            Languages = new Dictionary<string, string> { { "fr", "French" } }
        }
    };
        var jsonCountries = JsonConvert.SerializeObject(countries);

        _handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonCountries)
            });

        // Act
        var result = await _service.GetLanguagesAsync(true);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count); // Expecting two languages: "en" and "fr"
        Assert.Equal("en", result[0].Language);
        Assert.Equal("fr", result[1].Language);
        Assert.Equal(2, result[0].Countries.Count); // Two countries speak English
        Assert.Equal(2, result[1].Countries.Count); // Two countries speak French
    }

}
