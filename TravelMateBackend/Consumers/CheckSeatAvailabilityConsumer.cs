using MassTransit;
using TravelMate.Models.Messages;
using TravelMateBackend.Repositories.Offers;
using TravelMateBackend.Services.Offers;

public class CheckSeatAvailabilityConsumer : IConsumer<CheckSeatAvailabilityRequest>, IConsumer<CancelReservationRequest>
{
    private readonly IServiceProvider _serviceProvider;

    public CheckSeatAvailabilityConsumer(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    public async Task Consume(ConsumeContext<CheckSeatAvailabilityRequest> context)
    {
        var request = context.Message;
        Console.WriteLine("Received CheckSeatAvailabilityRequest: " + request.OfferId);
        using (var scope = _serviceProvider.CreateScope())
        {
            var offerService = scope.ServiceProvider.GetRequiredService<IOfferService>();
            var offerRepository = scope.ServiceProvider.GetRequiredService<IOfferRepository>();

            bool seatIsAvailable = await offerService.CheckSeatAvailability(request);

            await context.RespondAsync(new CheckSeatAvailabilityResponse
            {
                IsAvailable = seatIsAvailable
            });
        }
    }

    public async Task Consume(ConsumeContext<CancelReservationRequest> context)
    {
        var request = context.Message;
        Console.WriteLine("Received CancelReservation: " + request.OfferId);
        using (var scope = _serviceProvider.CreateScope())
        {
            var offerService = scope.ServiceProvider.GetRequiredService<IOfferService>();
            var offerRepository = scope.ServiceProvider.GetRequiredService<IOfferRepository>();

            await offerService.CancelSeatReservation(request);
            Console.WriteLine("Seat reservation cancelled for OfferId: " + request.OfferId);

            await context.RespondAsync(new CancelReservationResponse
            {
                IsCanceled = true
            });
        }
    }
}