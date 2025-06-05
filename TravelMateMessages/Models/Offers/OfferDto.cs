using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TravelMate.Models.Offers;

public class OfferDto
{
    [Key]
    public Guid Id { get; set; }
    public string AirplaneName { get; set; }
    public string AirlineName { get; set; }
    public string AirlineIconUrl { get; set; }
    public string DepartureAirportCode { get; set; }
    public string? DepartureAirportName { get; set; }
    public string DepartureAirportCity { get; set; }
    public string? DepartureAirportCountry { get; set; }
    public string ArrivalAirportCode { get; set; }
    public string? ArrivalAirportName { get; set; }
    public string ArrivalAirportCity { get; set; }
    public string? ArrivalAirportCountry { get; set; }
    public string FlightNumber { get; set; }
    public DateTime DepartureTime { get; set; }
    public DateTime ArrivalTime { get; set; }
    public decimal BasePrice { get; set; }
    public int AvailableEconomySeats { get; set; }
    public int AvailableBusinessSeats { get; set; }
    public int AvailableFirstClassSeats { get; set; }
    public DateTime CreatedAt { get; set; }
}
