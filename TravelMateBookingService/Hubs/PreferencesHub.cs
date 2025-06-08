using Microsoft.AspNetCore.SignalR;

namespace TravelMateBookingService.Hubs;

public class PreferencesHub : Hub
{
    public async Task SendUpdate(object data)
    {
        await Clients.All.SendAsync("ReceivePreferencesUpdate", data);
    }
}
