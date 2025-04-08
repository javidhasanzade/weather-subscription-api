using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using WeatherSubscriptionWebApp.Api.Controllers;
using WeatherSubscriptionWebApp.Application.Interfaces;
using WeatherSubscriptionWebApp.Domain.DTOs;
using WeatherSubscriptionWebApp.Domain.Entities;

namespace WeatherSubscriptionWebApp.Tests.ApiTests;

public class WeatherControllerTests
{
    [Fact]
        public async Task GetWeather_ShouldReturnOkResult_WithWeatherData()
        {
            // Arrange
            var subscription = new Subscription
            {
                Email = "user@example.com",
                City = "New York",
                Country = "United States",
                ZipCode = "10001",
                CountryCode = "US"
            };
            
            var expectedWeatherResponse = new WeatherResponse
            {
                WeatherDescription = "clear sky",
                Temperature = new TemperatureInfo { Current = 20, Min = 18, Max = 22 },
                Pressure = 1012,
                Humidity = 50,
                WindSpeed = 3.5,
                Cloudiness = "Clear",
                Sunrise = "06:00 AM",
                Sunset = "08:00 PM"
            };
            
            var subscriptionServiceMock = new Mock<ISubscriptionService>();
            subscriptionServiceMock
                .Setup(s => s.GetSubscriptionByEmailAsync("user@example.com"))
                .ReturnsAsync(subscription);

            var weatherServiceMock = new Mock<IWeatherService>();
            weatherServiceMock
                .Setup(ws => ws.GetWeatherByLocationAsync(subscription.City, subscription.Country, subscription.ZipCode, subscription.CountryCode))
                .ReturnsAsync(expectedWeatherResponse);

            var loggerMock = new Mock<ILogger<WeatherController>>();
            
            var controller = new WeatherController(subscriptionServiceMock.Object, weatherServiceMock.Object, loggerMock.Object);

            // Act
            IActionResult actionResult = await controller.GetWeather("user@example.com");

            // Assert
            actionResult.Should().BeOfType<OkObjectResult>();
            var okResult = actionResult as OkObjectResult;
            okResult.Value.Should().BeEquivalentTo(expectedWeatherResponse);
        }
}