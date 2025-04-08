using WeatherSubscriptionWebApp.Application.Interfaces;
using WeatherSubscriptionWebApp.Domain.DTOs;
using WeatherSubscriptionWebApp.Domain.Interfaces;

namespace WeatherSubscriptionWebApp.Application.Services;

public class WeatherService : IWeatherService
{
    private readonly IWeatherClient _weatherClient;

    public WeatherService(IWeatherClient weatherClient)
    {
        _weatherClient = weatherClient;
    }

    public async Task<WeatherResponse> GetWeatherByLocationAsync(string city, string country, string? zipCode = null, string? countryCode = null)
    {
        return await _weatherClient.GetWeatherDataAsync(city, country, zipCode, countryCode);
    }
}