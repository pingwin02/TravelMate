using TravelMateBookingService.Models.Bookings;

namespace TravelMate.Models.Messages;

public class CheckSeatAvailabilityRequest
{
    public Guid OfferId { get; set; }
    public SeatType SeatType { get; set; }
    public PassengerType PassengerType { get; set; }
}

public class CheckSeatAvailabilityResponse
{
    public bool IsAvailable { get; set; }
    public decimal DynamicPrice { get; set; }
}

public class CancelReservationRequest
{
    public Guid OfferId { get; set; }
    public SeatType SeatType { get; set; }
}

public class CancelReservationResponse
{
    public bool IsCanceled { get; set; }
}

public class PaymentRequest
{
    public Guid BookingId { get; set; }
    public decimal Price { get; set; }
}

public class PaymentResponse
{
    public Guid PaymentId { get; set; }
}