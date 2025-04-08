using System.Text.Json;
using Microsoft.Extensions.Configuration;
using WeatherSubscriptionWebApp.Domain.Interfaces;

namespace WeatherSubscriptionWebApp.Infrastructure.Services;

public class OpenWeatherMapGeoCodingService : IGeoCodingService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    
    public OpenWeatherMapGeoCodingService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["OpenWeatherMap:ApiKey"];
    }
    
    public async Task<(double lat, double lon)> GetCoordinatesAsync(string city, string country, string? zipCode = null, string? countryCode = null)
    {
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
            return (lat, lon);
        }
    }
}