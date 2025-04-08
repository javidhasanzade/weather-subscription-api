using FluentAssertions;
using Moq;
using WeatherSubscriptionWebApp.Application.Services;
using WeatherSubscriptionWebApp.Domain.Entities;
using WeatherSubscriptionWebApp.Domain.Interfaces;

namespace WeatherSubscriptionWebApp.Tests.ApplicationTests;

public class SubscriptionServiceTests
{
    private readonly Mock<ISubscriptionRepository> _subscriptionRepositoryMock;
        private readonly SubscriptionService _subscriptionService;

        public SubscriptionServiceTests()
        {
            _subscriptionRepositoryMock = new Mock<ISubscriptionRepository>();
            _subscriptionService = new SubscriptionService(_subscriptionRepositoryMock.Object);
        }

        [Fact]
        public async Task CreateSubscriptionAsync_ShouldAddSubscription_WhenEmailDoesNotExist()
        {
            // Arrange
            var subscription = new Subscription
            {
                Email = "newuser@example.com",
                Country = "United States",
                City = "New York",
                ZipCode = "10001",
                CountryCode = "US"
            };
            
            _subscriptionRepositoryMock.Setup(repo => repo.GetByEmailAsync(subscription.Email))
                .ReturnsAsync((Subscription)null);

            _subscriptionRepositoryMock.Setup(repo => repo.AddAsync(subscription))
                .Returns(Task.CompletedTask);

            // Act
            Func<Task> act = async () => await _subscriptionService.CreateSubscriptionAsync(subscription);

            // Assert
            await act.Should().NotThrowAsync();
            _subscriptionRepositoryMock.Verify(repo => repo.AddAsync(subscription), Times.Once);
        }

        [Fact]
        public async Task CreateSubscriptionAsync_ShouldThrowException_WhenEmailAlreadyExists()
        {
            // Arrange
            var subscription = new Subscription
            {
                Email = "existing@example.com",
                Country = "United States",
                City = "New York",
                ZipCode = "10001",
                CountryCode = "US"
            };
            
            _subscriptionRepositoryMock.Setup(repo => repo.GetByEmailAsync(subscription.Email))
                .ReturnsAsync(subscription);

            // Act
            Func<Task> act = async () => await _subscriptionService.CreateSubscriptionAsync(subscription);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("A subscription with this email already exists.");
            _subscriptionRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Subscription>()), Times.Never);
        }
}