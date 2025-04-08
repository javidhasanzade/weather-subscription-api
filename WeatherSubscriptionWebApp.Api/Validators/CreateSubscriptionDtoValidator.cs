using FluentValidation;
using WeatherSubscriptionWebApp.Api.DTOs;

namespace WeatherSubscriptionWebApp.Api.Validators;

public class CreateSubscriptionDtoValidator : AbstractValidator<CreateSubscriptionDto>
{
    public CreateSubscriptionDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email is required.");
        
        RuleFor(x => x.Country)
            .NotEmpty().WithMessage("Country is required.");
        
        RuleFor(x => x.City)
            .NotEmpty().WithMessage("City is required.");
        
        RuleFor(x => x.CountryCode)
            .Length(2).When(x => !string.IsNullOrEmpty(x.CountryCode))
            .WithMessage("CountryCode must be exactly 2 characters, e.g., 'US', 'GB'.");
    }
}