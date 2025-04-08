using WeatherSubscriptionWebApp.Domain.DTOs;

namespace WeatherSubscriptionWebApp.Domain.Interfaces;

public interface IWeatherResponseParser
{
    WeatherResponse Parse(string json);
}