namespace WeatherSubscriptionWebApp.Domain.Interfaces;

public interface IGeoCodingService
{
    /// <summary>
    /// Retrieves geographic coordinates for a given location.
    /// If ZIP code and country code are provided, use the ZIP API.
    /// Otherwise, use direct geocoding via city and full country name.
    /// </summary>
    Task<(double lat, double lon)> GetCoordinatesAsync(string city, string country, string? zipCode = null, string? countryCode = null);
}