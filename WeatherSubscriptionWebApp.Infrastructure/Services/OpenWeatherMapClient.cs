using System.Text.Json;
using Microsoft.Extensions.Configuration;
using WeatherSubscriptionWebApp.Domain.DTOs;
using WeatherSubscriptionWebApp.Domain.Interfaces;

namespace WeatherSubscriptionWebApp.Infrastructure.Services;

public class OpenWeatherMapClient : IWeatherClient
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly IGeoCodingService _geoCodingService;
    private readonly IWeatherResponseParser _responseParser;

    public OpenWeatherMapClient(HttpClient httpClient, IConfiguration configuration, IGeoCodingService geoCodingService, IWeatherResponseParser responseParser)
    {
        _httpClient = httpClient;
        _geoCodingService = geoCodingService;
        _responseParser = responseParser;
        _apiKey = configuration["OpenWeatherMap:ApiKey"];
    }

    public async Task<WeatherResponse> GetWeatherDataAsync(string city, string country, string? zipCode = null, string? countryCode = null)
        {
            var (lat, lon) = await _geoCodingService.GetCoordinatesAsync(city, country, zipCode, countryCode);
            string url = $"https://api.openweathermap.org/data/2.5/weather?lat={lat}&lon={lon}&appid={_apiKey}&units=metric";
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                throw new Exception("Error retrieving weather data from OpenWeatherMap.");
            string json = await response.Content.ReadAsStringAsync();
            return _responseParser.Parse(json);
        }
}