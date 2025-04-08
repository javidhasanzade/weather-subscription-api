using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using WeatherSubscriptionWebApp.Api.DTOs;
using WeatherSubscriptionWebApp.Application.Interfaces;
using WeatherSubscriptionWebApp.Domain.Entities;

namespace WeatherSubscriptionWebApp.Api.Controllers;

[ApiController]
[Route("api/v1/subscriptions")]
public class SubscriptionController : ControllerBase
{
    private readonly ILogger<SubscriptionController> _logger;
    private readonly ISubscriptionService _subscriptionService;
    private readonly IMapper _mapper;

    public SubscriptionController(ISubscriptionService subscriptionService, IMapper mapper, ILogger<SubscriptionController> logger)
    {
        _subscriptionService = subscriptionService;
        _mapper = mapper;
        _logger = logger;
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateSubscription([FromBody] CreateSubscriptionDto dto)
    {
        _logger.LogInformation("Received CreateSubscription request for email: {Email}", dto.Email);
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Model state invalid for CreateSubscription request.");
            return BadRequest(ModelState);
        }
        
        var subscription = _mapper.Map<Subscription>(dto);

        try
        {
            await _subscriptionService.CreateSubscriptionAsync(subscription);
            _logger.LogInformation("Subscription created successfully for email: {Email}", dto.Email);
            return CreatedAtAction(nameof(GetSubscription), new { email = subscription.Email }, subscription);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating subscription for email: {Email}", dto.Email);
            return BadRequest(new { message = ex.Message });
        }
    }

    // GET: api/v1/subscriptions/{email}
    [HttpGet("{email}")]
    public async Task<IActionResult> GetSubscription(string email)
    {
        _logger.LogInformation("Retrieving subscription for email: {Email}", email);
        var subscription = await _subscriptionService.GetSubscriptionByEmailAsync(email);
        if (subscription == null)
        {
            _logger.LogWarning("Subscription not found for email: {Email}", email);
            return NotFound(new { message = "Subscription not found." });
        }
        _logger.LogInformation("Subscription retrieved for email: {Email}", email);
        return Ok(subscription);
    }
}