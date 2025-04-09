using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using WeatherSubscriptionWebApp.Domain.Exceptions;
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
                var coordinates = await GetCoordinatesFromUrlAsync(url, ParseZipResponse);
                _logger.LogInformation("Coordinates from ZIP API: {Lat}, {Lon}", coordinates.lat, coordinates.lon);
                return coordinates;
            }
            else
            {
                string url = $"http://api.openweathermap.org/geo/1.0/direct?q={city},{country}&limit=1&appid={_apiKey}";
                var coordinates = await GetCoordinatesFromUrlAsync(url, ParseDirectGeoResponse);
                _logger.LogInformation("Coordinates from direct API: {Lat}, {Lon}", coordinates.lat, coordinates.lon);
                return coordinates;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving coordinates for City: {City} and Country: {Country}", city, country);
            throw;
        }
    }

    /// <summary>
    /// Helper method that performs the HTTP GET, verifies success, reads the response as JSON, and passes the JSON document
    /// to the provided parse function to extract coordinates.
    /// </summary>
    private async Task<(double lat, double lon)> GetCoordinatesFromUrlAsync(string url, Func<JsonDocument, (double lat, double lon)> parseCoordinates)
    {
        var response = await _httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Error retrieving location data from the API.");
        }
    
        string json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        return parseCoordinates(doc);
    }

    /// <summary>
    /// Parse function for the ZIP-based geocoding API response.
    /// Expected JSON format (object):
    /// {
    ///     "zip": "10001",
    ///     "name": "New York",
    ///     "lat": 40.7128,
    ///     "lon": -74.0060,
    ///     "country": "US"
    /// }
    /// </summary>
    private (double lat, double lon) ParseZipResponse(JsonDocument doc)
    {
        if (!doc.RootElement.TryGetProperty("lat", out JsonElement latElement) ||
            !doc.RootElement.TryGetProperty("lon", out JsonElement lonElement))
        {
            throw new WeatherDataNotFoundException("Invalid ZIP geocoding response: missing 'lat' or 'lon' properties.");
        }

        double lat = latElement.GetDouble();
        double lon = lonElement.GetDouble();
        return (lat, lon);
    }


    /// <summary>
    /// Parse function for the direct geocoding API response.
    /// Expected JSON format (array). Throws an exception if the array is empty.
    /// </summary>
    private (double lat, double lon) ParseDirectGeoResponse(JsonDocument doc)
    {
        if (doc.RootElement.GetArrayLength() == 0)
            throw new WeatherDataNotFoundException("Location not found using the direct geocoding API.");
        var location = doc.RootElement[0];
        double lat = location.GetProperty("lat").GetDouble();
        double lon = location.GetProperty("lon").GetDouble();
        return (lat, lon);
    }
}