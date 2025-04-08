using MediatR;
using WeatherSubscriptionWebApp.Domain.Entities;

namespace WeatherSubscriptionWebApp.Application.Queries;

public class GetSubscriptionByEmailQuery : IRequest<Subscription>
{
    public string Email { get; }

    public GetSubscriptionByEmailQuery(string email)
    {
        Email = email;
    }
}