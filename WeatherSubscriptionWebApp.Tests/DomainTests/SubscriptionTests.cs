using FluentAssertions;
using WeatherSubscriptionWebApp.Domain.Entities;

namespace WeatherSubscriptionWebApp.Tests.DomainTests;

public class SubscriptionTests
{
    [Fact]
    public void Creating_Subscription_With_Valid_Data_Should_Set_Properties()
    {
        // Arrange
        var email = "user@example.com";
        var country = "United States";
        var city = "New York";
        var zipCode = "10001";
        var countryCode = "US";
            
        // Act
        var subscription = new Subscription
        {
            Email = email,
            Country = country,
            City = city,
            ZipCode = zipCode,
            CountryCode = countryCode
        };

        // Assert
        subscription.Email.Should().Be(email);
        subscription.Country.Should().Be(country);
        subscription.City.Should().Be(city);
        subscription.ZipCode.Should().Be(zipCode);
        subscription.CountryCode.Should().Be(countryCode);
    }
}