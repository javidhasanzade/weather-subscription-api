using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using WeatherSubscriptionWebApp.Domain.Interfaces;

namespace WeatherSubscriptionWebApp.Infrastructure.Services;

public class OpenWeatherMapGeoCodingService : IGeoCodingService
{
    private readonly ILogger<OpenWeatherMapGeoCodingService> _logger;
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    
    public OpenWeatherMapGeoCodingService(HttpClient httpClient, IConfiguration configuration, ILogger<OpenWeatherMapGeoCodingService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _apiKey = configuration["OpenWeatherMap:ApiKey"];
    }
    
    public async Task<(double lat, double lon)> GetCoordinatesAsync(string city, string country, string? zipCode = null, string? countryCode = null)
    {
        try
        {
            _logger.LogInformation("Getting coordinates for City: {City}, Country: {Country}, Zip: {ZipCode}, CountryCode: {CountryCode}", city, country, zipCode, countryCode);
            if (!string.IsNullOrEmpty(zipCode) && !string.IsNullOrEmpty(countryCode))
            {
                string url = $"http://api.openweathermap.org/geo/1.0/zip?zip={zipCode},{countryCode}&appid={_apiKey}";
                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                    throw new Exception("Error retrieving location data using the ZIP geocoding API.");
                string json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                double lat = doc.RootElement.GetProperty("lat").GetDouble();
                double lon = doc.RootElement.GetProperty("lon").GetDouble();
                _logger.LogInformation("Coordinates from ZIP API: {Lat}, {Lon}", lat, lon);
                return (lat, lon);
            }
            else
            {
                string url = $"http://api.openweathermap.org/geo/1.0/direct?q={city},{country}&limit=1&appid={_apiKey}";
                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                    throw new Exception("Error retrieving location data using the direct geocoding API.");
                string json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                if (doc.RootElement.GetArrayLength() == 0)
                    throw new Exception("Location not found using the direct geocoding API.");
                var location = doc.RootElement[0];
                double lat = location.GetProperty("lat").GetDouble();
                double lon = location.GetProperty("lon").GetDouble();
                _logger.LogInformation("Coordinates from direct API: {Lat}, {Lon}", lat, lon);
                return (lat, lon);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving coordinates for City: {City} and Country: {Country}", city, country);
            throw;
        }
    }
}