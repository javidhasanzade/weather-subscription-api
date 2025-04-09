using System.Reflection;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using WeatherSubscriptionWebApp.Api.DTOs;
using WeatherSubscriptionWebApp.Api.Mappings;
using WeatherSubscriptionWebApp.Api.Middleware;
using WeatherSubscriptionWebApp.Api.Validators;
using WeatherSubscriptionWebApp.Application.Handlers;
using WeatherSubscriptionWebApp.Application.Interfaces;
using WeatherSubscriptionWebApp.Application.Services;
using WeatherSubscriptionWebApp.Domain.Interfaces;
using WeatherSubscriptionWebApp.Infrastructure.Data;
using WeatherSubscriptionWebApp.Infrastructure.Repositories;
using WeatherSubscriptionWebApp.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", false, true);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

// Add services to the container.
builder.Host.UseSerilog();
builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHttpClient<IWeatherClient, OpenWeatherMapClient>();
builder.Services.AddAutoMapper(typeof(SubscriptionMappingProfile));
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(CreateSubscriptionCommandHandler).Assembly));

builder.Services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();
builder.Services.AddScoped<IWeatherService, WeatherService>();
builder.Services.AddScoped<IGeoCodingService, OpenWeatherMapGeoCodingService>();
builder.Services.AddScoped<IWeatherResponseParser, OpenWeatherMapResponseParser>();
builder.Services.AddTransient<IValidator<CreateSubscriptionDto>, CreateSubscriptionDtoValidator>();

builder.Services.AddMemoryCache();
builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "WeatherSubscriptionAPI", Version = "v1" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<ErrorHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;

try
{
    var context = services.GetRequiredService<AppDbContext>();
    await context.Database.MigrateAsync();
}
catch (Exception e)
{
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError(e, "An error occured during migration");
    throw;
}

app.Run();