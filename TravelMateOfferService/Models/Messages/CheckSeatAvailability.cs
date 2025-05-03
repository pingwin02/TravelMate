using TravelMateOfferService.Models;

namespace TravelMate.Models.Messages;

public class CheckSeatAvailabilityRequest
{
    public Guid OfferId { get; set; }
    public SeatType SeatType { get; set; }
}

public class CheckSeatAvailabilityResponse
{
    public bool IsAvailable { get; set; }
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