using TravelMatePaymentService.Models.Bookings;

namespace TravelMate.Models.Messages;

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