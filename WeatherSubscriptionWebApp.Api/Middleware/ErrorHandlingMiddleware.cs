using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using WeatherSubscriptionWebApp.Domain.Exceptions;

namespace WeatherSubscriptionWebApp.Api.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;
    private readonly IHostEnvironment _environment;
    
    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger, IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }
    
    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        int statusCode;
        string message;
        
        switch (exception)
        {
            case SubscriptionAlreadyExistsException sae:
                statusCode = StatusCodes.Status400BadRequest;
                message = sae.Message;
                break;
            case WeatherDataNotFoundException wdnf:
                statusCode = StatusCodes.Status404NotFound;
                message = wdnf.Message;
                break;
            case ValidationException ve:
                statusCode = StatusCodes.Status400BadRequest;
                message = ve.Message;
                break;
            case UnauthorizedAccessException _:
                statusCode = StatusCodes.Status401Unauthorized;
                message = "Unauthorized access.";
                break;
            default:
                statusCode = StatusCodes.Status500InternalServerError;
                message = "An unexpected error occurred.";
                break;
        }
        
        if (_environment.IsDevelopment())
        {
            message = exception.Message;
        }
        
        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = message,
            Instance = context.Request.Path,
        };
        
        problemDetails.Extensions["correlationId"] = context.TraceIdentifier;
        
        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = statusCode;
        
        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var jsonResponse = JsonSerializer.Serialize(problemDetails, options);
        
        _logger.LogError(exception, "An error occurred (correlationId: {CorrelationId})", context.TraceIdentifier);

        await context.Response.WriteAsync(jsonResponse);
    }
}