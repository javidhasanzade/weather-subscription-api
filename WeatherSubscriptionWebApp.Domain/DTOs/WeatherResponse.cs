namespace WeatherSubscriptionWebApp.Domain.DTOs;

public class TemperatureInfo
{
    public double Current { get; set; }
    public double Min { get; set; }
    public double Max { get; set; }
}

public class WeatherResponse
{
    public string WeatherDescription { get; set; }
    public TemperatureInfo Temperature { get; set; }
    public int Pressure { get; set; }
    public int Humidity { get; set; }
    public double WindSpeed { get; set; }
    public string Cloudiness { get; set; }
    public string Sunrise { get; set; }
    public string Sunset { get; set; }
}