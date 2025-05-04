using MassTransit;
using TravelMate.Models.Messages;
using TravelMateOfferService.Services;

namespace TravelMateOfferService.Consumers;

public class CheckSeatAvailabilityConsumer(IServiceProvider serviceProvider)
    : IConsumer<CheckSeatAvailabilityRequest>, IConsumer<CancelReservationRequest>
{
    public async Task Consume(ConsumeContext<CancelReservationRequest> context)
    {
        var request = context.Message;
        Console.WriteLine("Received CancelReservation: " + request.OfferId);
        using var scope = serviceProvider.CreateScope();
        var offerService = scope.ServiceProvider.GetRequiredService<IOfferService>();

        await offerService.CancelSeatReservation(request);
        Console.WriteLine("Seat reservation cancelled for OfferId: " + request.OfferId);

        await context.RespondAsync(new CancelReservationResponse
        {
            IsCanceled = true
        });
    }

    public async Task Consume(ConsumeContext<CheckSeatAvailabilityRequest> context)
    {
        var request = context.Message;
        Console.WriteLine("Received CheckSeatAvailabilityRequest: " + request.OfferId);
        using var scope = serviceProvider.CreateScope();
        var offerService = scope.ServiceProvider.GetRequiredService<IOfferService>();

        var seatIsAvailable = await offerService.CheckSeatAvailability(request);

        await context.RespondAsync(new CheckSeatAvailabilityResponse
        {
            IsAvailable = seatIsAvailable,
            DynamicPrice = seatIsAvailable ? await offerService.CalculateDynamicPrice(request) : 0
        });
    }
}