using TravelMatePaymentService.Models.Bookings;

namespace TravelMate.Models.Messages;

public class PaymentCreationRequest
{
    public Guid CorrelationId { get; set; }
    public Guid BookingId { get; set; }
    public decimal Price { get; set; }
}

public class PaymentCreatedEvent
{
    public Guid CorrelationId { get; set; }
    public Guid PaymentId { get; set; }
}

public class BookingStatusUpdateRequest
{
    public Guid CorrelationId { get; set; }
    public Guid BookingId { get; set; }
    public BookingStatus Status { get; set; }
}

public class BookingStatusUpdateResponse
{
    public Guid CorrelationId { get; set; }
    public bool IsUpdated { get; set; }
}

public class PaymentCreationEvent
{
    public Guid CorrelationId { get; set; }
    public Guid PaymentId { get; set; }
}

public class PaymentFinalizedEvent
{
    public Guid CorrelationId { get; set; }
    public bool IsSuccess { get; set; }
}