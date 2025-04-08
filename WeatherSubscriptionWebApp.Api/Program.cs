using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using WeatherSubscriptionWebApp.Application.Interfaces;
using WeatherSubscriptionWebApp.Application.Services;
using WeatherSubscriptionWebApp.Domain.Interfaces;
using WeatherSubscriptionWebApp.Infrastructure.Data;
using WeatherSubscriptionWebApp.Infrastructure.Repositories;
using WeatherSubscriptionWebApp.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", false, true);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHttpClient<IWeatherClient, OpenWeatherMapClient>();

builder.Services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();
builder.Services.AddScoped<IWeatherService, WeatherService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "WeatherSubscriptionAPI", Version = "v1" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();