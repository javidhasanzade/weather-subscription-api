using WeatherSubscriptionWebApp.Domain.DTOs;

namespace WeatherSubscriptionWebApp.Application.Interfaces;

public interface IWeatherService
{
    /// <summary>
    /// Retrieves current weather data based on city, country, and optionally zip code.
    /// </summary>
    Task<WeatherResponse> GetWeatherByLocationAsync(string city, string country, string zipCode = null);
}