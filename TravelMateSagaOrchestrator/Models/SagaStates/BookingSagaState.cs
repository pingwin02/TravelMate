using MassTransit;
using TravelMate.Models.Messages;

namespace TravelMateSagaOrchestrator.Models.SagaStates;

public class BookingSagaState : SagaStateMachineInstance
{
    public Guid BookingId { get; set; }
    public Guid PaymentId { get; set; }
    public SeatType SeatType { get; set; }
    public PassengerType PassengerType { get; set; }
    public string CurrentState { get; set; } = null!;
    public DateTime Created { get; set; }
    public decimal Price { get; set; }
    public Guid OfferId { get; set; }
    public Guid CorrelationId { get; set; }


}