using Microsoft.Extensions.Logging;
using WeatherSubscriptionWebApp.Application.Interfaces;
using WeatherSubscriptionWebApp.Domain.DTOs;
using WeatherSubscriptionWebApp.Domain.Interfaces;

namespace WeatherSubscriptionWebApp.Application.Services;

public class WeatherService : IWeatherService
{
    private readonly ILogger<WeatherService> _logger;
    private readonly IWeatherClient _weatherClient;

    public WeatherService(IWeatherClient weatherClient, ILogger<WeatherService> logger)
    {
        _weatherClient = weatherClient;
        _logger = logger;
    }

    public async Task<WeatherResponse> GetWeatherByLocationAsync(string city, string country, string? zipCode = null, string? countryCode = null)
    {
        _logger.LogInformation("Fetching weather for {City}, {Country} with ZIP {Zip} and CountryCode {CountryCode}", city, country, zipCode, countryCode);
        try
        {
            var weatherResponse = await _weatherClient.GetWeatherDataAsync(city, country, zipCode, countryCode);
            _logger.LogInformation("Successfully retrieved weather data for {City}, {Country}", city, country);
            return weatherResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving weather data for {City}, {Country}", city, country);
            throw;
        }
    }
}