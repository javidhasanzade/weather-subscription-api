using MediatR;
using WeatherSubscriptionWebApp.Domain.Entities;

namespace WeatherSubscriptionWebApp.Application.Commands;

public class CreateSubscriptionCommand : IRequest<Subscription>
{
    public string Email { get; set; }
    public string Country { get; set; }
    public string City { get; set; }
    public string? ZipCode { get; set; }
    public string? CountryCode { get; set; }
}