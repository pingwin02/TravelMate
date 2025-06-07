using MassTransit;
using Microsoft.AspNetCore.SignalR;
using TravelMate.Models.Messages;
using TravelMateOfferQueryService.Hubs;

namespace TravelMateOfferCommandService.Consumers;

public class PurchaseNotificationEventConsumer(IServiceProvider serviceProvider)
    : IConsumer<PurchaseNotificationEvent>
{
    public async Task Consume(ConsumeContext<PurchaseNotificationEvent> context)
    {
        var request = context.Message;
        Console.WriteLine("Received Purchase Notification Event: " + request.OfferId);
        using var scope = serviceProvider.CreateScope();
        var hub = scope.ServiceProvider.GetRequiredService<IHubContext<OfferHub>>();

        await hub.Clients.Group(request.OfferId.ToString())
            .SendAsync("OfferPurchased");
    }
}