namespace TravelMateBookingService.Models.Bookings.DTO;

public class BookingDto
{
    public Guid Id { get; set; }
    public Guid PaymentId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ReservedUntil { get; set; }
}