using System.ComponentModel.DataAnnotations;

namespace TravelMateOfferCommandService.Models;

public class Airport
{
    [Key] [StringLength(3)] public string Code { get; set; }
    public string? Name { get; set; }
    public string City { get; set; }
    public string? Country { get; set; }
}