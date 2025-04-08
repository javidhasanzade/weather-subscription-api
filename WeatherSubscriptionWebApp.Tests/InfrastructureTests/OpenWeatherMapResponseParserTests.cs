using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using WeatherSubscriptionWebApp.Domain.DTOs;
using WeatherSubscriptionWebApp.Infrastructure.Services;

namespace WeatherSubscriptionWebApp.Tests.InfrastructureTests;

public class OpenWeatherMapResponseParserTests
{
    [Fact]
    public void Parse_ShouldReturnValidWeatherResponse()
    {
        // Arrange
        string json = @"{
                ""weather"": [ { ""description"": ""clear sky"" } ],
                ""main"": {
                    ""temp"": 20.0,
                    ""temp_min"": 18.0,
                    ""temp_max"": 22.0,
                    ""pressure"": 1012,
                    ""humidity"": 50
                },
                ""wind"": { ""speed"": 3.5 },
                ""clouds"": { ""all"": 10 },
                ""sys"": {
                    ""sunrise"": 1600000000,
                    ""sunset"": 1600040000
                }
            }";

        var loggerMock = new Mock<ILogger<OpenWeatherMapResponseParser>>();
        var parser = new OpenWeatherMapResponseParser(loggerMock.Object);

        // Act
        WeatherResponse response = parser.Parse(json);

        // Assert
        response.WeatherDescription.Should().Be("clear sky");
        response.Temperature.Current.Should().Be(20.0);
        response.Temperature.Min.Should().Be(18.0);
        response.Temperature.Max.Should().Be(22.0);
        response.Pressure.Should().Be(1012);
        response.Humidity.Should().Be(50);
        response.WindSpeed.Should().Be(3.5);
        response.Cloudiness.Should().Be("Clear");  // assuming ConvertCloudiness logic maps 10% to "Clear"
        response.Sunrise.Should().NotBeNull();
        response.Sunset.Should().NotBeNull();
    }
}