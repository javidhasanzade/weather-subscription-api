using AutoMapper;
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
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<SubscriptionController>> _loggerMock;
        private readonly SubscriptionController _controller;

        public SubscriptionsControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<SubscriptionController>>();

            _controller = new SubscriptionController(_loggerMock.Object, _mediatorMock.Object,  _mapperMock.Object);
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
            
            var responseDto = new SubscriptionResponseDto
            {
                Email = createdSubscription.Email,
                Country = createdSubscription.Country,
                City = createdSubscription.City,
                ZipCode = createdSubscription.ZipCode,
            };
            
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<CreateSubscriptionCommand>(), default))
                .ReturnsAsync(createdSubscription);
            
            _mapperMock
                .Setup(m => m.Map<SubscriptionResponseDto>(It.IsAny<Subscription>()))
                .Returns(responseDto);

            // Act
            var result = await _controller.CreateSubscription(dto);

            // Assert
            result.Should().BeOfType<CreatedAtActionResult>();
            var createdResult = result as CreatedAtActionResult;
            createdResult.RouteValues["email"].Should().Be(responseDto.Email);
            createdResult.Value.Should().BeEquivalentTo(responseDto);
        }

        [Fact]
        public async Task GetSubscription_ShouldReturnOk_WhenSubscriptionExists()
        {
            // Arrange
            var email = "test@example.com";
            var subscription = new Subscription
            {
                Id = 1,
                Email = email,
                Country = "United States",
                City = "New York",
                ZipCode = "10001",
                CountryCode = "US"
            };
            
            var responseDto = new SubscriptionResponseDto
            {
                Email = subscription.Email,
                Country = subscription.Country,
                City = subscription.City,
                ZipCode = subscription.ZipCode,
            };
            
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetSubscriptionByEmailQuery>(), default))
                .ReturnsAsync(subscription);
            
            _mapperMock
                .Setup(m => m.Map<SubscriptionResponseDto>(It.IsAny<Subscription>()))
                .Returns(responseDto);

            // Act
            var result = await _controller.GetSubscription(email);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.Value.Should().BeEquivalentTo(responseDto);
        }

        [Fact]
        public async Task GetSubscription_ShouldReturnNotFound_WhenSubscriptionDoesNotExist()
        {
            // Arrange
            var email = "nonexistent@example.com";
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetSubscriptionByEmailQuery>(), default))
                .ReturnsAsync((Subscription)null);

            // Act
            var result = await _controller.GetSubscription(email);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }
}