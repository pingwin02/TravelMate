using MassTransit;
using TravelMate.Models.Messages;
using TravelMateOfferService.Services;

namespace TravelMateOfferService.Consumers;

public class CheckSeatAvailabilityConsumer(IServiceProvider serviceProvider)
    : IConsumer<CheckSeatAvailabilityRequest>
{
    public async Task Consume(ConsumeContext<CheckSeatAvailabilityRequest> context)
    {
        var request = context.Message;
        Console.WriteLine("Received CheckSeatAvailabilityRequest: " + request.OfferId);
        using var scope = serviceProvider.CreateScope();
        var offerService = scope.ServiceProvider.GetRequiredService<IOfferService>();

        var seatIsAvailable = await offerService.CheckSeatAvailability(request);

        await context.Publish(new CheckSeatAvailabilityResponse
        {
            CorrelationId = context.Message.CorrelationId,
            IsAvailable = seatIsAvailable,
            DynamicPrice = seatIsAvailable ? await offerService.CalculateDynamicPrice(request) : 0
        });
    }
}