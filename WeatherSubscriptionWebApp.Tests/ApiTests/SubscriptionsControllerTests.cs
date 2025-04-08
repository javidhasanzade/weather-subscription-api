using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using WeatherSubscriptionWebApp.Api.Controllers;
using WeatherSubscriptionWebApp.Api.DTOs;
using WeatherSubscriptionWebApp.Application.Commands;
using WeatherSubscriptionWebApp.Application.Queries;
using WeatherSubscriptionWebApp.Domain.Entities;

namespace WeatherSubscriptionWebApp.Tests.ApiTests;

public class SubscriptionsControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<ILogger<SubscriptionController>> _loggerMock;
    private readonly SubscriptionController _controller;

    public SubscriptionsControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _loggerMock = new Mock<ILogger<SubscriptionController>>();
        _controller = new SubscriptionController(_loggerMock.Object, _mediatorMock.Object);
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
        
        var createdSubscription = new Subscription
        {
            Id = 1, // Normally assigned by the database.
            Email = dto.Email,
            Country = dto.Country,
            City = dto.City,
            ZipCode = dto.ZipCode,
            CountryCode = dto.CountryCode
        };
        
        _mediatorMock
            .Setup(x => x.Send(It.IsAny<CreateSubscriptionCommand>(), default))
            .ReturnsAsync(createdSubscription);

        // Act
        var result = await _controller.CreateSubscription(dto);

        // Assert
        result.Should().BeOfType<CreatedAtActionResult>();
        var createdAtActionResult = result as CreatedAtActionResult;
        createdAtActionResult.RouteValues["email"].Should().Be(createdSubscription.Email);
    }

    [Fact]
    public async Task GetSubscription_ShouldReturnOk_WhenSubscriptionExists()
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
        
        _mediatorMock
            .Setup(x => x.Send(It.IsAny<GetSubscriptionByEmailQuery>(), default))
            .ReturnsAsync(subscription);

        // Act
        var result = await _controller.GetSubscription(email);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult.Value.Should().BeEquivalentTo(subscription);
    }

    [Fact]
    public async Task GetSubscription_ShouldReturnNotFound_WhenSubscriptionDoesNotExist()
    {
        // Arrange
        var email = "nonexistent@example.com";
        _mediatorMock
            .Setup(x => x.Send(It.IsAny<GetSubscriptionByEmailQuery>(), default))
            .ReturnsAsync((Subscription)null);

        // Act
        var result = await _controller.GetSubscription(email);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }
}