using System.ComponentModel.DataAnnotations;

namespace TravelMateOfferCommandService.Models;

public class Airline
{
    [Key] public string Name { get; set; }
    public string IconUrl { get; set; }
}