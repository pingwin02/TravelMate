using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelMateBookingService.Models.Bookings;

public class Booking
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public Guid OfferId { get; set; }
    public Guid? PaymentId { get; set; }
    public BookingStatus Status { get; set; }
    public DateTime ReservedUntil { get; set; }
    public SeatType SeatType { get; set; }
    public string PassengerName { get; set; }
    public PassengerType PassengerType { get; set; }
    public DateTime CreatedAt { get; set; }
}

public enum BookingStatus
{
    Pending,
    Confirmed,
    Canceled
}

public enum SeatType
{
    Economy,
    Business,
    FirstClass
}

public enum PassengerType
{
    Adult,
    Child,
    Baby
}