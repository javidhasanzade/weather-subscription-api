using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using WeatherSubscriptionWebApp.Application.Interfaces;
using WeatherSubscriptionWebApp.Domain.DTOs;
using WeatherSubscriptionWebApp.Domain.Interfaces;

namespace WeatherSubscriptionWebApp.Application.Services;

public class WeatherService : IWeatherService
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<WeatherService> _logger;
    private readonly IWeatherClient _weatherClient;

    public WeatherService(IWeatherClient weatherClient, ILogger<WeatherService> logger, IMemoryCache cache)
    {
        _weatherClient = weatherClient;
        _logger = logger;
        _cache = cache;
    }

    public async Task<WeatherResponse> GetWeatherByLocationAsync(string city, string country, string? zipCode = null, string? countryCode = null)
    {
        var cacheKey = $"weather_{city}_{country}_{zipCode ?? "null"}_{countryCode ?? "null"}";

        if (_cache.TryGetValue(cacheKey, out WeatherResponse cachedWeather))
        {
            _logger.LogInformation("Cache hit for key: {CacheKey}", cacheKey);
            return cachedWeather;
        }

        _logger.LogInformation("Cache miss for key: {CacheKey}. Fetching weather data...", cacheKey);
        
        var weatherResponse = await _weatherClient.GetWeatherDataAsync(city, country, zipCode, countryCode);
        
        var cacheEntryOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
        };
        
        _cache.Set(cacheKey, weatherResponse, cacheEntryOptions);
        _logger.LogInformation("Weather data cached with key: {CacheKey}", cacheKey);

        return weatherResponse;
    }
}