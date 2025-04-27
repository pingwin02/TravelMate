namespace TravelMateBookingService.Models.Bookings.DTO
{
    public class BookingRequestDto
    {
        public Guid OfferId { get; set; }
        public Guid UserId { get; set; }
        public string SeatNumber { get; set; }
        public SeatType SeatType { get; set; }
        public string PassengerName { get; set; }
        public PassengerType PassengerType { get; set; }
    }
}
