namespace WeatherSubscriptionWebApp.Domain.Exceptions;

public class SubscriptionAlreadyExistsException : Exception
{
    public SubscriptionAlreadyExistsException(string email)
        : base($"A subscription with the email '{email}' already exists.")
    {
    }
}