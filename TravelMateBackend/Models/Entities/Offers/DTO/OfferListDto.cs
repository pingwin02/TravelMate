namespace TravelMateBackend.Models.Entities.Offers.DTO
{
    public class OfferListDto
    {
        public Guid Id { get; set; }
        public string AirlineName { get; set; }
        public string FlightNumber { get; set; }
        public string DepartureAirport { get; set; }
        public string ArrivalAirport { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public decimal BasePrice { get; set; }
    }
}
