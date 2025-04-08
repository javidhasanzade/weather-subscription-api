using System.ComponentModel.DataAnnotations;

namespace WeatherSubscriptionWebApp.Domain.Entities;

public class Subscription
{
    [Key]
    public int Id { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }
    
    [Required]
    public string Country { get; set; }
    
    [Required]
    public string City { get; set; }
    
    public string ZipCode { get; set; }
}