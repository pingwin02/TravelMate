using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using TravelMateBackend.Models.Entities;

namespace Models.Entities.Offers
{
    public class Offer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Airplane Airplane { get; set; }
        public Airline Airline { get; set; }
        public Airport DepartureAirport { get; set; }
        public Airport ArrivalAirport { get; set; }
        public string FlightNumber { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public decimal BasePrice { get; set; }
        public int AvailableEconomySeats { get; set; }
        public int AvailableBusinessSeats { get; set; }
        public int AvailableFirstClassSeats { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
