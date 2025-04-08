using WeatherSubscriptionWebApp.Domain.Entities;

namespace WeatherSubscriptionWebApp.Application.Interfaces;

public interface ISubscriptionService
{
    /// <summary>
    /// Creates a new subscription if one doesn't exist with the same email.
    /// </summary>
    /// <param name="subscription">The subscription entity to be created.</param>
    Task CreateSubscriptionAsync(Subscription subscription);
    
    /// <summary>
    /// Retrieves a subscription by email.
    /// </summary>
    /// <param name="email">The email identifier of the subscription.</param>
    /// <returns>The matching subscription, or null if not found.</returns>
    Task<Subscription> GetSubscriptionByEmailAsync(string email);
}