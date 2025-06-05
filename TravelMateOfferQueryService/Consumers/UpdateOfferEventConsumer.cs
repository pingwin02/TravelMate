using MassTransit;
using TravelMate.Models.Messages;
using TravelMateOfferQueryService.Services;

namespace TravelMateOfferQueryService.Consumers;

public class UpdateOfferEventConsumer(IServiceProvider serviceProvider) : IConsumer<UpdateOfferEvent>
{
    public async Task Consume(ConsumeContext<UpdateOfferEvent> context)
    {
        var request = context.Message;
        Console.WriteLine("Received Update Offer Event, Offer id: " + request.Offer.Id);
        using var scope = serviceProvider.CreateScope();
        var offerQueryService = scope.ServiceProvider.GetRequiredService<IOfferQueryService>();

        await offerQueryService.UpdateOffer(request.Offer);
        Console.WriteLine("Offer updated in query db, offer id: " + request.Offer.Id);
    }
}