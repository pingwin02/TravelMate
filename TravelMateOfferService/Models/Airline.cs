using System.ComponentModel.DataAnnotations;

namespace TravelMateOfferService.Models
{
    public class Airline
    {
        [Key] public string Name { get; set; }
        public string IconUrl { get; set; }
    }
}