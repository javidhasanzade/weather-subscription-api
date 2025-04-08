using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using WeatherSubscriptionWebApp.Api.Controllers;
using WeatherSubscriptionWebApp.Api.DTOs;
using WeatherSubscriptionWebApp.Api.Mappings;
using WeatherSubscriptionWebApp.Application.Interfaces;
using WeatherSubscriptionWebApp.Domain.Entities;

namespace WeatherSubscriptionWebApp.Tests.ApiTests;

public class SubscriptionsControllerTests
{
    private readonly Mock<ISubscriptionService> _subscriptionServiceMock;
        private readonly IMapper _mapper;
        private readonly Mock<ILogger<SubscriptionController>> _loggerMock;
        private readonly SubscriptionController _controller;

        public SubscriptionsControllerTests()
        {
            _subscriptionServiceMock = new Mock<ISubscriptionService>();
            _loggerMock = new Mock<ILogger<SubscriptionController>>();
            
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<SubscriptionMappingProfile>();
            });
            _mapper = mapperConfig.CreateMapper();

            _controller = new SubscriptionController(_subscriptionServiceMock.Object, _mapper, _loggerMock.Object);
        }

        [Fact]
        public async Task CreateSubscription_ShouldReturnCreatedAtActionResult_WhenSubscriptionIsValid()
        {
            // Arrange
            var dto = new CreateSubscriptionDto
            {
                Email = "test@example.com",
                Country = "United States",
                City = "New York",
                ZipCode = "10001",
                CountryCode = "US"
            };
            
            _subscriptionServiceMock.Setup(x => x.CreateSubscriptionAsync(It.IsAny<Subscription>()))
                .Returns(Task.CompletedTask);
            
            _subscriptionServiceMock.Setup(x => x.GetSubscriptionByEmailAsync("test@example.com"))
                .ReturnsAsync(new Subscription
                {
                    Email = "test@example.com",
                    Country = "United States",
                    City = "New York",
                    ZipCode = "10001",
                    CountryCode = "US"
                });

            // Act
            IActionResult result = await _controller.CreateSubscription(dto);

            // Assert
            result.Should().BeOfType<CreatedAtActionResult>();
            var createdResult = result as CreatedAtActionResult;
            createdResult.RouteValues["email"].Should().Be("test@example.com");
        }

        [Fact]
        public async Task GetSubscription_ShouldReturnOkObjectResult_WhenSubscriptionExists()
        {
            // Arrange
            var email = "test@example.com";
            var subscription = new Subscription
            {
                Email = email,
                Country = "United States",
                City = "New York",
                ZipCode = "10001",
                CountryCode = "US"
            };

            _subscriptionServiceMock.Setup(x => x.GetSubscriptionByEmailAsync(email))
                .ReturnsAsync(subscription);

            // Act
            IActionResult result = await _controller.GetSubscription(email);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.Value.Should().BeEquivalentTo(subscription);
        }

        [Fact]
        public async Task GetSubscription_ShouldReturnNotFound_WhenSubscriptionDoesNotExist()
        {
            // Arrange
            var email = "notfound@example.com";
            _subscriptionServiceMock.Setup(x => x.GetSubscriptionByEmailAsync(email))
                .ReturnsAsync((Subscription)null);

            // Act
            IActionResult result = await _controller.GetSubscription(email);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }
}