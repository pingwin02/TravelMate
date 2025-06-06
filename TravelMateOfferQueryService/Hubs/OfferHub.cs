using Microsoft.AspNetCore.SignalR;

namespace TravelMateOfferQueryService.Hubs;

public class OfferHub : Hub
{
    public async Task JoinOfferGroup(string offerId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, offerId);
        await Clients.Group(offerId).SendAsync("UserJoined", $"User {Context.ConnectionId} joined offer {offerId}");
    }   
    public async Task LeaveOfferGroup(string offerId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, offerId);
        await Clients.Group(offerId).SendAsync("UserLeft", $"User {Context.ConnectionId} left offer {offerId}");
    }
}
