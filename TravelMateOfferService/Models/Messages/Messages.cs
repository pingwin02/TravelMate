using TravelMateOfferService.Models;

namespace TravelMate.Models.Messages;

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

public class CancelPaymentCommand
{
    public Guid CorrelationId { get; set; }
    public Guid PaymentId { get; set; }
}
public class CancelSeatAvailabilityCommand
{
    public Guid CorrelationId { get; set; }
    public Guid OfferId { get; set; }
    public SeatType SeatType { get; set; }
}