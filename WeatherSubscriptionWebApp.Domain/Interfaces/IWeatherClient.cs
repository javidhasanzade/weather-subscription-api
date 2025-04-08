using WeatherSubscriptionWebApp.Domain.DTOs;

namespace WeatherSubscriptionWebApp.Domain.Interfaces;

public interface IWeatherClient
{
    /// <summary>
    /// Retrieves weather data from the external OpenWeatherMap API.
    /// </summary>
    Task<WeatherResponse> GetWeatherDataAsync(string city, string country, string zipCode = null);
}