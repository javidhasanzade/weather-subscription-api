using MediatR;
using WeatherSubscriptionWebApp.Application.Commands;
using WeatherSubscriptionWebApp.Domain.Entities;
using WeatherSubscriptionWebApp.Domain.Exceptions;
using WeatherSubscriptionWebApp.Domain.Interfaces;

namespace WeatherSubscriptionWebApp.Application.Handlers;

public class CreateSubscriptionCommandHandler : IRequestHandler<CreateSubscriptionCommand, Subscription>
{
    private readonly ISubscriptionRepository _subscriptionRepository;

    public CreateSubscriptionCommandHandler(ISubscriptionRepository subscriptionRepository)
    {
        _subscriptionRepository = subscriptionRepository;
    }

    public async Task<Subscription> Handle(CreateSubscriptionCommand request, CancellationToken cancellationToken)
    {
        var existingSubscription = await _subscriptionRepository.GetByEmailAsync(request.Email);
        if (existingSubscription != null)
        {
            throw new SubscriptionAlreadyExistsException(request.Email);
        }
        
        var newSubscription = new Subscription
        {
            Email = request.Email,
            Country = request.Country,
            City = request.City,
            ZipCode = request.ZipCode,
            CountryCode = request.CountryCode
        };

        await _subscriptionRepository.AddAsync(newSubscription);
        return newSubscription;
    }
}