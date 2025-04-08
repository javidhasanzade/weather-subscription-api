using System.Text.Json;
using Microsoft.Extensions.Logging;
using WeatherSubscriptionWebApp.Domain.DTOs;
using WeatherSubscriptionWebApp.Domain.Interfaces;

namespace WeatherSubscriptionWebApp.Infrastructure.Services;

public class OpenWeatherMapResponseParser : IWeatherResponseParser
{
    private readonly ILogger<OpenWeatherMapResponseParser> _logger;

    public OpenWeatherMapResponseParser(ILogger<OpenWeatherMapResponseParser> logger)
    {
        _logger = logger;
    }

    public WeatherResponse Parse(string json)
    {
        try
        {
            _logger.LogInformation("Parsing weather response JSON.");
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            string weatherDescription = root.GetProperty("weather")[0].GetProperty("description").GetString();
            var main = root.GetProperty("main");
            double temp = main.GetProperty("temp").GetDouble();
            double tempMin = main.GetProperty("temp_min").GetDouble();
            double tempMax = main.GetProperty("temp_max").GetDouble();
            int pressure = main.GetProperty("pressure").GetInt32();
            int humidity = main.GetProperty("humidity").GetInt32();

            var wind = root.GetProperty("wind");
            double windSpeed = wind.GetProperty("speed").GetDouble();

            var clouds = root.GetProperty("clouds");
            int cloudinessPercent = clouds.GetProperty("all").GetInt32();
            string cloudiness = ConvertCloudiness(cloudinessPercent);

            var sys = root.GetProperty("sys");
            long sunriseUnix = sys.GetProperty("sunrise").GetInt64();
            long sunsetUnix = sys.GetProperty("sunset").GetInt64();
            string sunrise = DateTimeOffset.FromUnixTimeSeconds(sunriseUnix).ToLocalTime().ToString("hh:mm tt");
            string sunset = DateTimeOffset.FromUnixTimeSeconds(sunsetUnix).ToLocalTime().ToString("hh:mm tt");

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
        catch(Exception ex)
        {
            _logger.LogError(ex, "An error occurred while parsing the weather response.");
            throw;
        }
    }

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