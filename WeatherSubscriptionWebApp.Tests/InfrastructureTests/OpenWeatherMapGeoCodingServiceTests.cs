using System.Net;
using System.Text;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using WeatherSubscriptionWebApp.Infrastructure.Services;

namespace WeatherSubscriptionWebApp.Tests.InfrastructureTests;

public class OpenWeatherMapGeoCodingServiceTests
{
    [Fact]
    public async Task GetCoordinatesAsync_ShouldReturnCoordinates_ForZipBasedCall()
    {
        // Arrange
        var fakeJson = @"{""zip"":""10001"",""name"":""New York"",""lat"":40.7128,""lon"":-74.0060,""country"":""US""}";
        var fakeResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(fakeJson, Encoding.UTF8, "application/json")
        };

        var handler = new FakeHttpMessageHandler(fakeResponse);
        var httpClient = new HttpClient(handler);

        var inMemorySettings = new Dictionary<string, string>
        {
            { "OpenWeatherMap:ApiKey", "fake-api-key" }
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        var loggerMock = new Mock<ILogger<OpenWeatherMapGeoCodingService>>();

        var service = new OpenWeatherMapGeoCodingService(httpClient, configuration, loggerMock.Object);

        // Act
        var (lat, lon) = await service.GetCoordinatesAsync("New York", "United States", "10001", "US");

        // Assert
        lat.Should().BeApproximately(40.7128, 0.0001);
        lon.Should().BeApproximately(-74.0060, 0.0001);
    }
}