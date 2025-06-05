namespace TravelMateOfferCommandService.Models.DTO;

public class OfferRequestDto
{
    public Guid AirplaneId { get; set; }
    public string AirlineName { get; set; }
    public string DepartureAirportCode { get; set; }
    public string ArrivalAirportCode { get; set; }
    public string FlightNumber { get; set; }
    public DateTime DepartureTime { get; set; }
    public DateTime ArrivalTime { get; set; }
    public decimal BasePrice { get; set; }
    public int AvailableEconomySeats { get; set; }
    public int AvailableBusinessSeats { get; set; }
    public int AvailableFirstClassSeats { get; set; }
}