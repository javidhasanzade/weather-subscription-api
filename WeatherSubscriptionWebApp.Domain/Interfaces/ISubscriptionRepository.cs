using WeatherSubscriptionWebApp.Domain.Entities;

namespace WeatherSubscriptionWebApp.Domain.Interfaces;

public interface ISubscriptionRepository
{
    /// <summary>
    /// Adds a new subscription asynchronously.
    /// </summary>
    /// <param name="subscription">The subscription to add.</param>
    Task AddAsync(Subscription subscription);
    
    /// <summary>
    /// Retrieves a subscription by email asynchronously.
    /// </summary>
    /// <param name="email">The email address to look up.</param>
    /// <returns>The matching subscription, or null if not found.</returns>
    Task<Subscription> GetByEmailAsync(string email);
}