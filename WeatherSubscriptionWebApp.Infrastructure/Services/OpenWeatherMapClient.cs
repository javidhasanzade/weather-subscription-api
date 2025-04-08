using System.Text.Json;
using Microsoft.Extensions.Configuration;
using WeatherSubscriptionWebApp.Domain.DTOs;
using WeatherSubscriptionWebApp.Domain.Interfaces;

namespace WeatherSubscriptionWebApp.Infrastructure.Services;

public class OpenWeatherMapClient : IWeatherClient
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public OpenWeatherMapClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["OpenWeatherMap:ApiKey"];
    }

    public async Task<WeatherResponse> GetWeatherDataAsync(string city, string country, string zipCode = null)
    {
        double lat, lon;

        if (!string.IsNullOrEmpty(zipCode))
        {
            // Use the ZIP code geocoding API.
            string zipGeoUrl = $"http://api.openweathermap.org/geo/1.0/zip?zip={zipCode},{country}&appid={_apiKey}";
            var zipGeoResponse = await _httpClient.GetAsync(zipGeoUrl);
            if (!zipGeoResponse.IsSuccessStatusCode)
            {
                throw new Exception("Error retrieving location data using the ZIP geocoding API.");
            }
            var zipGeoJson = await zipGeoResponse.Content.ReadAsStringAsync();
            using var zipGeoDoc = JsonDocument.Parse(zipGeoJson);
            lat = zipGeoDoc.RootElement.GetProperty("lat").GetDouble();
            lon = zipGeoDoc.RootElement.GetProperty("lon").GetDouble();
        }
        else
        {
            // Use the direct geocoding API for city and country.
            string directGeoUrl = $"http://api.openweathermap.org/geo/1.0/direct?q={city},{country}&limit=1&appid={_apiKey}";
            var directGeoResponse = await _httpClient.GetAsync(directGeoUrl);
            if (!directGeoResponse.IsSuccessStatusCode)
            {
                throw new Exception("Error retrieving location data using the direct geocoding API.");
            }
            var directGeoJson = await directGeoResponse.Content.ReadAsStringAsync();
            using var directGeoDoc = JsonDocument.Parse(directGeoJson);
            if (directGeoDoc.RootElement.GetArrayLength() == 0)
            {
                throw new Exception("Location not found using the direct geocoding API.");
            }
            var locationElem = directGeoDoc.RootElement[0];
            lat = locationElem.GetProperty("lat").GetDouble();
            lon = locationElem.GetProperty("lon").GetDouble();
        }
        
        // With coordinates, call the weather API.
        string weatherUrl = $"https://api.openweathermap.org/data/2.5/weather?lat={lat}&lon={lon}&appid={_apiKey}&units=metric";
        var weatherResponse = await _httpClient.GetAsync(weatherUrl);
        if (!weatherResponse.IsSuccessStatusCode)
        {
            throw new Exception("Error retrieving weather data from OpenWeatherMap.");
        }
        var weatherJson = await weatherResponse.Content.ReadAsStringAsync();

        using var weatherDoc = JsonDocument.Parse(weatherJson);
        var root = weatherDoc.RootElement;

        // Extract weather description.
        string weatherDescription = root.GetProperty("weather")[0].GetProperty("description").GetString();

        // Parse temperature and other main metrics.
        var main = root.GetProperty("main");
        double temp = main.GetProperty("temp").GetDouble();
        double tempMin = main.GetProperty("temp_min").GetDouble();
        double tempMax = main.GetProperty("temp_max").GetDouble();
        int pressure = main.GetProperty("pressure").GetInt32();
        int humidity = main.GetProperty("humidity").GetInt32();

        // Parse wind data.
        var wind = root.GetProperty("wind");
        double windSpeed = wind.GetProperty("speed").GetDouble();

        // Parse cloud information and convert to descriptive text.
        var clouds = root.GetProperty("clouds");
        int cloudinessPercent = clouds.GetProperty("all").GetInt32();
        string cloudiness = ConvertCloudiness(cloudinessPercent);

        // Parse sunrise and sunset times.
        var sys = root.GetProperty("sys");
        long sunriseUnix = sys.GetProperty("sunrise").GetInt64();
        long sunsetUnix = sys.GetProperty("sunset").GetInt64();

        string sunrise = DateTimeOffset.FromUnixTimeSeconds(sunriseUnix)
            .ToLocalTime().ToString("hh:mm tt");
        string sunset = DateTimeOffset.FromUnixTimeSeconds(sunsetUnix)
            .ToLocalTime().ToString("hh:mm tt");

        return new WeatherResponse
        {
            WeatherDescription = weatherDescription,
            Temperature = new TemperatureInfo
            {
                Current = temp,
                Min = tempMin,
                Max = tempMax
            },
            Pressure = pressure,
            Humidity = humidity,
            WindSpeed = windSpeed,
            Cloudiness = cloudiness,
            Sunrise = sunrise,
            Sunset = sunset 
        };
    }
    
    /// <summary>
    /// Converts a cloudiness percentage into human-friendly text.
    /// </summary>
    private string ConvertCloudiness(int percentage)
    {
        if (percentage < 20)
            return "Clear";
        else if (percentage < 50)
            return "Partly Cloudy";
        else if (percentage < 80)
            return "Mostly Cloudy";
        else
            return "Overcast";
    }
}