using AutoMapper;
using WeatherSubscriptionWebApp.Api.DTOs;
using WeatherSubscriptionWebApp.Domain.Entities;

namespace WeatherSubscriptionWebApp.Api.Mappings;

public class SubscriptionMappingProfile : Profile
{
    public SubscriptionMappingProfile()
    {
        CreateMap<CreateSubscriptionDto, Subscription>();
    }
}