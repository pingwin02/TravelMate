using TravelMateBookingService.Models.Bookings;

namespace TravelMate.Models.Messages;

public class BookingStartedEvent
{
    public Guid CorrelationId { get; set; }
    public Guid BookingId { get; set; }
    public Guid OfferId { get; set; }
    public SeatType SeatType { get; set; }
    public PassengerType PassengerType { get; set; }
}

public class CheckSeatAvailabilityRequest
{
    public Guid CorrelationId { get; set; }
    public Guid OfferId { get; set; }
    public SeatType SeatType { get; set; }
    public PassengerType PassengerType { get; set; }
}

public class CheckSeatAvailabilityResponse
{
    public Guid CorrelationId { get; set; }
    public bool IsAvailable { get; set; }
    public decimal DynamicPrice { get; set; }
}

public class CancelReservationRequest
{
    public Guid CorrelationId { get; set; }
    public Guid OfferId { get; set; }
    public SeatType SeatType { get; set; }
}

public class CancelReservationResponse
{
    public Guid CorrelationId { get; set; }
    public bool IsCanceled { get; set; }
}

public class PaymentCreationRequest
{
    public Guid BookingId { get; set; }
    public decimal Price { get; set; }
}

public class PaymentCreationResponse
{
    public Guid PaymentId { get; set; }
}

public class BookingStatusUpdateRequest
{
    public Guid BookingId { get; set; }
    public BookingStatus Status { get; set; }
}

public class BookingStatusUpdateResponse
{
    public bool IsUpdated { get; set; }
}
public class BookingSagaStatusResponse
{
    public Guid CorrelationId { get; set; }
    public Guid PaymentId { get; set; }
    public bool IsSuccessful { get; set; }

}