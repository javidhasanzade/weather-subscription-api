using Microsoft.EntityFrameworkCore;
using WeatherSubscriptionWebApp.Domain.Entities;
using WeatherSubscriptionWebApp.Domain.Interfaces;
using WeatherSubscriptionWebApp.Infrastructure.Data;

namespace WeatherSubscriptionWebApp.Infrastructure.Repositories;

public class SubscriptionRepository : ISubscriptionRepository
{
    private readonly AppDbContext _dbContext;

    public SubscriptionRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Subscription subscription)
    {
        _dbContext.Subscriptions.Add(subscription);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<Subscription> GetByEmailAsync(string email)
    {
        return await _dbContext.Subscriptions.FirstOrDefaultAsync(s => s.Email == email);
    }
}