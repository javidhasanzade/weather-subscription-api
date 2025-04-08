using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using WeatherSubscriptionWebApp.Domain.DTOs;
using WeatherSubscriptionWebApp.Domain.Interfaces;

namespace WeatherSubscriptionWebApp.Infrastructure.Services;

public class OpenWeatherMapClient : IWeatherClient
{
    private readonly ILogger<OpenWeatherMapClient> _logger;
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly IGeoCodingService _geoCodingService;
    private readonly IWeatherResponseParser _responseParser;

    public OpenWeatherMapClient(HttpClient httpClient, IConfiguration configuration, IGeoCodingService geoCodingService, IWeatherResponseParser responseParser, ILogger<OpenWeatherMapClient> logger)
    {
        _httpClient = httpClient;
        _geoCodingService = geoCodingService;
        _responseParser = responseParser;
        _logger = logger;
        _apiKey = configuration["OpenWeatherMap:ApiKey"];
    }

    public async Task<WeatherResponse> GetWeatherDataAsync(string city, string country, string? zipCode = null, string? countryCode = null)
        {
            try
            {
                _logger.LogInformation("Getting weather data for City: {City}, Country: {Country}, Zip: {ZipCode}, CountryCode: {CountryCode}", city, country, zipCode, countryCode);
                var (lat, lon) = await _geoCodingService.GetCoordinatesAsync(city, country, zipCode, countryCode);
                _logger.LogInformation("Coordinates obtained: {Lat}, {Lon}", lat, lon);

                string url = $"https://api.openweathermap.org/data/2.5/weather?lat={lat}&lon={lon}&appid={_apiKey}&units=metric";
                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                    throw new Exception("Error retrieving weather data from OpenWeatherMap.");

                string json = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Weather response JSON: {Json}", json.Substring(0, Math.Min(json.Length, 200))); // Log first 200 chars for brevity.
                return _responseParser.Parse(json);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get weather data for City: {City}, Country: {Country}", city, country);
                throw;
            }
        }
}