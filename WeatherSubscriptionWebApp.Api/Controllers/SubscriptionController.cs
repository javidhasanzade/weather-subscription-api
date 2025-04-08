using Microsoft.AspNetCore.Mvc;
using WeatherSubscriptionWebApp.Api.DTOs;
using WeatherSubscriptionWebApp.Application.Interfaces;
using WeatherSubscriptionWebApp.Domain.Entities;

namespace WeatherSubscriptionWebApp.Api.Controllers;

[ApiController]
[Route("api/v1/subscriptions")]
public class SubscriptionController : ControllerBase
{
    private readonly ISubscriptionService _subscriptionService;

    public SubscriptionController(ISubscriptionService subscriptionService)
    {
        _subscriptionService = subscriptionService;
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateSubscription([FromBody] CreateSubscriptionDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        var subscription = new Subscription
        {
            Email = dto.Email,
            Country = dto.Country,
            City = dto.City,
            ZipCode = dto.ZipCode
        };

        try
        {
            await _subscriptionService.CreateSubscriptionAsync(subscription);
            return CreatedAtAction(nameof(GetSubscription), new { email = subscription.Email }, subscription);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // GET: api/v1/subscriptions/{email}
    [HttpGet("{email}")]
    public async Task<IActionResult> GetSubscription(string email)
    {
        var subscription = await _subscriptionService.GetSubscriptionByEmailAsync(email);
        if (subscription == null)
            return NotFound(new { message = "Subscription not found." });
        return Ok(subscription);
    }
}