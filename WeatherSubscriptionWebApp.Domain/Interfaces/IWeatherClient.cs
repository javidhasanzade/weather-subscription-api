using WeatherSubscriptionWebApp.Domain.DTOs;

namespace WeatherSubscriptionWebApp.Domain.Interfaces;

public interface IWeatherClient
{
    /// <summary>
    /// Retrieves weather data from OpenWeatherMap.
    /// If both zipCode and countryCode are provided, the ZIP geocoding API will be used.
    /// Otherwise, the direct geocoding API via city and country is used.
    /// </summary>
    Task<WeatherResponse> GetWeatherDataAsync(string city, string country, string? zipCode = null, string? countryCode = null);
}