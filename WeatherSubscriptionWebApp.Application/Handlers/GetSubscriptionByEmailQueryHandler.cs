using MediatR;
using WeatherSubscriptionWebApp.Application.Queries;
using WeatherSubscriptionWebApp.Domain.Entities;
using WeatherSubscriptionWebApp.Domain.Interfaces;

namespace WeatherSubscriptionWebApp.Application.Handlers;

public class GetSubscriptionByEmailQueryHandler : IRequestHandler<GetSubscriptionByEmailQuery, Subscription>
{
    private readonly ISubscriptionRepository _subscriptionRepository;

    public GetSubscriptionByEmailQueryHandler(ISubscriptionRepository subscriptionRepository)
    {
        _subscriptionRepository = subscriptionRepository;
    }

    public async Task<Subscription> Handle(GetSubscriptionByEmailQuery request, CancellationToken cancellationToken)
    {
        return await _subscriptionRepository.GetByEmailAsync(request.Email);
    }
}