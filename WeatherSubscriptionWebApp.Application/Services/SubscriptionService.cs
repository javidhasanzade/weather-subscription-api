using WeatherSubscriptionWebApp.Application.Interfaces;
using WeatherSubscriptionWebApp.Domain.Entities;
using WeatherSubscriptionWebApp.Domain.Interfaces;

namespace WeatherSubscriptionWebApp.Application.Services;

public class SubscriptionService : ISubscriptionService
{
    private readonly ISubscriptionRepository _subscriptionRepository;

    public SubscriptionService(ISubscriptionRepository subscriptionRepository)
    {
        _subscriptionRepository = subscriptionRepository;
    }

    public async Task CreateSubscriptionAsync(Subscription subscription)
    {
        var existingSubscription = await _subscriptionRepository.GetByEmailAsync(subscription.Email);
        if (existingSubscription != null)
        {
            throw new Exception("A subscription with this email already exists.");
        }
        
        await _subscriptionRepository.AddAsync(subscription);
    }

    public async Task<Subscription> GetSubscriptionByEmailAsync(string email)
    {
        return await _subscriptionRepository.GetByEmailAsync(email);
    }
}