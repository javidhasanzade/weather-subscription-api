using Microsoft.AspNetCore.Mvc;
using WeatherSubscriptionWebApp.Application.Interfaces;

namespace WeatherSubscriptionWebApp.Api.Controllers;

[ApiController]
[Route("api/v1/weather")]
public class WeatherController : ControllerBase
{
    private readonly ISubscriptionService _subscriptionService;
    private readonly IWeatherService _weatherService;

    public WeatherController(
        ISubscriptionService subscriptionService,
        IWeatherService weatherService)
    {
        _subscriptionService = subscriptionService;
        _weatherService = weatherService;
    }

    // GET: api/v1/weather?email=user@example.com
    [HttpGet]
    public async Task<IActionResult> GetWeather([FromQuery] string email)
    {
        if (string.IsNullOrEmpty(email))
            return BadRequest(new { message = "Email is required for login." });

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
            return Ok(weather);
        }
        catch (System.Exception ex)
        {
            return StatusCode(502, new { message = ex.Message });
        }
    }
}