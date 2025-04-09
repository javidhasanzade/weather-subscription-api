using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WeatherSubscriptionWebApp.Api.DTOs;
using WeatherSubscriptionWebApp.Application.Commands;
using WeatherSubscriptionWebApp.Application.Queries;
using WeatherSubscriptionWebApp.Domain.Entities;

namespace WeatherSubscriptionWebApp.Api.Controllers;

[ApiController]
[Route("api/v1/subscriptions")]
public class SubscriptionController : ControllerBase
{
    private readonly ILogger<SubscriptionController> _logger;
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public SubscriptionController(ILogger<SubscriptionController> logger, IMediator mediator, IMapper mapper)
    {
        _logger = logger;
        _mediator = mediator;
        _mapper = mapper;
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateSubscription([FromBody] CreateSubscriptionDto dto)
    {
        _logger.LogInformation("Received CreateSubscription request for email: {Email}", dto.Email);
        try
        {
            var command = new CreateSubscriptionCommand
            {
                Email = dto.Email,
                Country = dto.Country,
                City = dto.City,
                ZipCode = dto.ZipCode,
                CountryCode = dto.CountryCode
            };

            Subscription createdSubscription = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetSubscription), new { email = createdSubscription.Email }, createdSubscription);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating subscription for email: {Email}", dto.Email);
            return BadRequest(new { message = ex.Message });
        }
    }
    
    [HttpGet("{email}")]
    public async Task<IActionResult> GetSubscription(string email)
    {
        _logger.LogInformation("Received GetSubscription request for email: {Email}", email);
        var query = new GetSubscriptionByEmailQuery(email);
        Subscription subscription = await _mediator.Send(query);
        if (subscription == null)
        {
            return NotFound(new { message = "Subscription not found." });
        }
        SubscriptionResponseDto responseDto = _mapper.Map<SubscriptionResponseDto>(subscription);
        return Ok(responseDto);
    }
}