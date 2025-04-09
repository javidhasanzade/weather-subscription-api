namespace WeatherSubscriptionWebApp.Api.DTOs;

public class SubscriptionResponseDto
{
    public string Email { get; set; }
    public string Country { get; set; }
    public string City { get; set; }
    public string? ZipCode { get; set; }
}