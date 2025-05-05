using MassTransit;
using TravelMate.Models.Messages;
using TravelMateSagaOrchestrator.Models.SagaStates;

namespace TravelMateSagaOrchestrator.Saga;

public class BookingSaga : MassTransitStateMachine<BookingSagaState>
{
    public BookingSaga()
    {
        InstanceState(x => x.CurrentState);

        Event(() => BookingStarted, x => x.CorrelateById(m => m.Message.CorrelationId));
        Event(() => SeatAvailabilityChecked, x => x.CorrelateById(m => m.Message.CorrelationId));
        Event(() => PaymentCreatedEvent, x => x.CorrelateById(m => m.Message.CorrelationId));
        Event(() => PaymentFinalizedEvent, x => x.CorrelateById(m => m.Message.CorrelationId));
        Event(() => BookingCancelledEvent, x => x.CorrelateById(m => m.Message.CorrelationId));
        Event(() => PaymentFailedEvent, x => x.CorrelateById(m => m.Message.CorrelationId));

        Initially(
            When(BookingStarted)
                .Then(context =>
                {
                    context.Saga.BookingId = context.Message.BookingId;
                    context.Saga.OfferId = context.Message.OfferId;
                    context.Saga.Created = DateTime.Now;
                    context.Saga.SeatType = context.Message.SeatType;
                    context.Saga.PassengerType = context.Message.PassengerType;
                    Console.WriteLine($"[Saga] Booking started with ID {context.Saga.BookingId}");
                })
                .SendAsync(new Uri("queue:check-seat-availability-queue"), context =>
                    Task.FromResult(new CheckSeatAvailabilityRequest
                    {
                        CorrelationId = context.Saga.CorrelationId,
                        OfferId = context.Saga.OfferId,
                        SeatType = context.Saga.SeatType,
                        PassengerType = context.Saga.PassengerType
                    }))
        );

        During(Initial,
            When(SeatAvailabilityChecked)
                .IfElse(context => context.Message.IsAvailable,
                    thenBinder => thenBinder
                        .Then(context =>
                        {
                            Console.WriteLine($"[Saga] Seat available for offer {context.Saga.OfferId}");
                        })
                        .TransitionTo(SeatChecked)
                        .SendAsync(new Uri("queue:create-payment"), context =>
                            Task.FromResult(new PaymentCreationRequest
                            {
                                CorrelationId = context.Saga.CorrelationId,
                                BookingId = context.Saga.BookingId,
                                Price = context.Saga.Price
                            })),
                    elseBinder => elseBinder
                        .Then(async context =>
                        {
                            Console.WriteLine(
                                $"[Saga] No available seat for offer {context.Saga.OfferId}. Booking failed.");
                            //await PerformSeatCompensation(context);
                        })
                        .Finalize()
                )
        );

        During(SeatChecked,
            When(PaymentCreatedEvent)
                .Then(context =>
                {
                    context.Saga.PaymentId = context.Message.PaymentId;
                    Console.WriteLine($"[Saga] Payment created: {context.Saga.PaymentId}");
                })
                .SendAsync(context=>new Uri($"queue:booking-status-response-{context.Saga.CorrelationId}"), context =>
                    Task.FromResult(new BookingSagaStatusResponse
                    {
                        CorrelationId = context.Saga.CorrelationId,
                        PaymentId = context.Saga.PaymentId,
                        IsSuccessful = true
                    }))
                .TransitionTo(PaymentCreated)
        );

        During(PaymentCreated,

            When(PaymentFinalizedEvent)
                .Then(context =>
                {
                    Console.WriteLine(
                        $"[Saga] Payment finalized for correlation {context.Saga.CorrelationId} with status: {(context.Message.IsSuccess ? "Success" : "Failure")}");

                    Console.WriteLine("[Saga] Updating booking status");
                })
                .SendAsync(new Uri("queue:update-booking-status-queue"), context =>
                            Task.FromResult(new BookingStatusUpdateRequest
                            {
                                CorrelationId = context.Saga.CorrelationId,
                                BookingId = context.Saga.BookingId,
                                Status = BookingStatus.Confirmed
                            }))
                .TransitionTo(PaymentFinalized)
                .Finalize()
        );

        DuringAny(
            When(BookingCancelledEvent)
                .ThenAsync(async context =>
                {
                    Console.WriteLine($"[Saga] Booking cancelled for correlation {context.Saga.CorrelationId}");
                    await PerformBookingCancellationCompesation(context);
                })
                .Finalize(),
            When(PaymentFailedEvent)
                .ThenAsync(async context =>
                {
                    Console.WriteLine($"[Saga] Payment failed for correlation {context.Saga.CorrelationId}");
                    await PerformPaymentFailureCompensation(context);
                })
                .Finalize()
        );

        SetCompletedWhenFinalized();
    }

    private async Task PerformBookingCancellationCompesation(SagaConsumeContext<BookingSagaState, BookingCancelledEvent> context)
    {
        Console.WriteLine($"current state {context.Saga.CurrentState}");
        if (context.Saga.CurrentState.Equals(SeatChecked.Name))
        {
            Console.WriteLine($"[Saga] Compensating seat availability for booking {context.Saga.BookingId}");
            await context.Send(new Uri("queue:cancel-seat-availability-queue"), new CancelSeatAvailabilityCommand
            {
                CorrelationId = context.Saga.CorrelationId,
                OfferId = context.Saga.OfferId,
                SeatType = context.Saga.SeatType
            });

            return;
        }
        if (context.Saga.CurrentState.Equals(PaymentCreated.Name))
        {
            Console.WriteLine($"[Saga] Compensating payment for booking {context.Saga.BookingId}");
            await context.Send(new Uri("queue:cancel-payment-queue"), new CancelPaymentCommand
            {
                CorrelationId = context.Saga.CorrelationId,
                PaymentId = context.Saga.PaymentId
            });
            Console.WriteLine($"[Saga] Compensating seat availability for booking {context.Saga.BookingId}");
            await context.Send(new Uri("queue:cancel-seat-availability-queue"), new CancelSeatAvailabilityCommand
            {
                CorrelationId = context.Saga.CorrelationId,
                OfferId = context.Saga.OfferId,
                SeatType = context.Saga.SeatType
            });
        }
        
    }

    private async Task PerformPaymentFailureCompensation(SagaConsumeContext<BookingSagaState, PaymentFailedEvent> context)
    {
        Console.WriteLine($"[Saga] Compensating seat availability for booking {context.Saga.BookingId}");
        await context.Send(new Uri("queue:cancel-seat-availability-queue"), new CancelSeatAvailabilityCommand
        {
            CorrelationId = context.Saga.CorrelationId,
            OfferId = context.Saga.OfferId,
            SeatType = context.Saga.SeatType
        });

        Console.WriteLine($"[Saga] Cancelling booking {context.Saga.BookingId}");
        await context.Send(new Uri("queue:cancel-booking-queue"), new CancelBookingCommand
        {
            CorrelationId = context.Saga.CorrelationId,
            BookingId = context.Saga.BookingId
        });
    }




    public State SeatChecked { get; } = null!;
    public State PaymentCreated { get; } = null!;
    public State PaymentFinalized { get; } = null!;

    public Event<BookingStartedEvent> BookingStarted { get; } = null!;
    public Event<CheckSeatAvailabilityResponse> SeatAvailabilityChecked { get; } = null!;
    public Event<PaymentCreatedEvent> PaymentCreatedEvent { get; } = null!;
    public Event<PaymentFinalizedEvent> PaymentFinalizedEvent { get; } = null!;
    public Event<BookingCancelledEvent> BookingCancelledEvent { get; } = null!;
    public Event<PaymentFailedEvent> PaymentFailedEvent { get; } = null!;


}

