using TravelMateSagaOrchestrator.Models.SagaStates;

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


public class PaymentFinalizedEvent
{
    public Guid CorrelationId { get; set; }
    public bool IsSuccess { get; set; }
}
public class PaymentCreatedEvent
{
    public Guid CorrelationId { get; set; }
    public Guid PaymentId { get; set; }
}



    public class PaymentCreationRequest
{
    public Guid CorrelationId { get; set; }
    public Guid BookingId { get; set; }
    public decimal Price { get; set; }
}

public class BookingStatusUpdateRequest
{
    public Guid BookingId { get; set; }
    public BookingStatus Status { get; set; }
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