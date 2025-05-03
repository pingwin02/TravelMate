namespace TravelMateBookingService.Models.Bookings.DTO;

public class BookingDto
{
    public Guid Id { get; set; }
    public Guid OfferId { get; set; }
    public string PaymentUrl { get; set; }
    public BookingStatus Status { get; set; }
    public string SeatNumber { get; set; }
    public SeatType SeatType { get; set; }
    public string PassengerName { get; set; }
    public PassengerType PassengerType { get; set; }
    public DateTime CreatedAt { get; set; }
}