using Microsoft.AspNetCore.Mvc;
using WeatherSubscriptionWebApp.Application.Interfaces;

namespace WeatherSubscriptionWebApp.Api.Controllers;

[ApiController]
[Route("api/v1/weather")]
public class WeatherController : ControllerBase
{
    private readonly ILogger<WeatherController> _logger;
    private readonly ISubscriptionService _subscriptionService;
    private readonly IWeatherService _weatherService;

    public WeatherController(
        ISubscriptionService subscriptionService,
        IWeatherService weatherService, ILogger<WeatherController> logger)
    {
        _subscriptionService = subscriptionService;
        _weatherService = weatherService;
        _logger = logger;
    }

    // GET: api/v1/weather?email=user@example.com
    [HttpGet]
    public async Task<IActionResult> GetWeather([FromQuery] string email)
    {
        _logger.LogInformation("Received GetWeather request for email: {Email}", email);

        if (string.IsNullOrEmpty(email))
        {
            _logger.LogWarning("Email parameter missing in GetWeather request.");
            return BadRequest(new { message = "Email is required for login." });
        }

        var subscription = await _subscriptionService.GetSubscriptionByEmailAsync(email);
        if (subscription == null)
            return NotFound(new { message = "Subscription not found for the provided email." });

        try
        {
            var weather = await _weatherService.GetWeatherByLocationAsync(
                subscription.City,
                subscription.Country,
                subscription.ZipCode,
                subscription.CountryCode);
            _logger.LogInformation("Returning weather data for email: {Email}", email);
            return Ok(weather);
        }
        catch (System.Exception ex)
        {
            _logger.LogError(ex, "Error processing GetWeather for email: {Email}", email);
            return StatusCode(502, new { message = ex.Message });
        }
    }
}