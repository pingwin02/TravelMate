using MassTransit;
using TravelMate.Models.Messages;
using TravelMateOfferCommandService.Services;

namespace TravelMateOfferCommandService.Consumers;

public class CancelSeatAvailabilityConsumer(IServiceProvider serviceProvider) : IConsumer<CancelSeatAvailabilityCommand>
{
    public async Task Consume(ConsumeContext<CancelSeatAvailabilityCommand> context)
    {
        var request = context.Message;
        Console.WriteLine("Received CancelReservation: " + request.OfferId);
        using var scope = serviceProvider.CreateScope();
        var offerService = scope.ServiceProvider.GetRequiredService<IOfferService>();

        await offerService.CancelSeatReservation(request);
        Console.WriteLine("Seat reservation cancelled for OfferId: " + request.OfferId);
    }
}