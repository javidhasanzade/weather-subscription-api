using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using WeatherSubscriptionWebApp.Application.Interfaces;
using WeatherSubscriptionWebApp.Application.Services;
using WeatherSubscriptionWebApp.Domain.DTOs;
using WeatherSubscriptionWebApp.Domain.Interfaces;

namespace WeatherSubscriptionWebApp.Tests.ApplicationTests;

public class WeatherServiceTests
{
    private readonly Mock<IWeatherClient> _weatherClientMock;
    private readonly IMemoryCache _memoryCache;
    private readonly Mock<ILogger<WeatherService>> _loggerMock;
    private readonly IWeatherService _weatherService;

    public WeatherServiceTests()
    {
        _weatherClientMock = new Mock<IWeatherClient>();
        _memoryCache = new MemoryCache(new MemoryCacheOptions());
        _loggerMock = new Mock<ILogger<WeatherService>>();
        _weatherService = new WeatherService(_weatherClientMock.Object, _loggerMock.Object, _memoryCache);
    }

    [Fact]
    public async Task GetWeatherByLocationAsync_ShouldReturnData_WhenCacheIsMiss()
    {
        // Arrange
        var city = "New York";
        var country = "United States";
        string zipCode = "10001";
        string countryCode = "US";

        var expectedResponse = new WeatherResponse
        {
            WeatherDescription = "Clear sky",
            Temperature = new TemperatureInfo { Current = 25, Min = 20, Max = 28 },
            Pressure = 1015,
            Humidity = 50,
            WindSpeed = 5,
            Cloudiness = "Clear",
            Sunrise = "06:00 AM",
            Sunset = "08:00 PM"
        };

        _weatherClientMock
            .Setup(x => x.GetWeatherDataAsync(city, country, zipCode, countryCode))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _weatherService.GetWeatherByLocationAsync(city, country, zipCode, countryCode);

        // Assert
        result.Should().BeEquivalentTo(expectedResponse);
        _weatherClientMock.Verify(x => x.GetWeatherDataAsync(city, country, zipCode, countryCode), Times.Once);
    }

    [Fact]
    public async Task GetWeatherByLocationAsync_ShouldReturnCachedData_OnSubsequentCalls()
    {
        // Arrange
        var city = "New York";
        var country = "United States";
        string zipCode = "10001";
        string countryCode = "US";

        var expectedResponse = new WeatherResponse
        {
            WeatherDescription = "Sunny",
            Temperature = new TemperatureInfo { Current = 30, Min = 25, Max = 35 },
            Pressure = 1010,
            Humidity = 40,
            WindSpeed = 7,
            Cloudiness = "Clear",
            Sunrise = "06:15 AM",
            Sunset = "08:10 PM"
        };

        _weatherClientMock
            .Setup(x => x.GetWeatherDataAsync(city, country, zipCode, countryCode))
            .ReturnsAsync(expectedResponse);

        // First call – cache miss, calls the client.
        var result1 = await _weatherService.GetWeatherByLocationAsync(city, country, zipCode, countryCode);

        // Second call – should hit the cache.
        var result2 = await _weatherService.GetWeatherByLocationAsync(city, country, zipCode, countryCode);

        // Assert
        result1.Should().BeEquivalentTo(expectedResponse);
        result2.Should().BeEquivalentTo(expectedResponse);
        // Verify that the client was called only once.
        _weatherClientMock.Verify(x => x.GetWeatherDataAsync(city, country, zipCode, countryCode), Times.Once);
    }
}