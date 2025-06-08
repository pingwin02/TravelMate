using MassTransit;
using TravelMate.Models.Messages;
using TravelMate.Models.Offers;
using TravelMateBookingService.Services;

namespace TravelMateBookingService.Consumers;

public class CancelBookingConsumer(IServiceProvider serviceProvider) : IConsumer<CancelBookingCommand>
{
    public async Task Consume(ConsumeContext<CancelBookingCommand> context)
    {
        var request = context.Message;
        Console.WriteLine("Received CancelBooking: " + request.BookingId);
        using var scope = serviceProvider.CreateScope();
        var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();

        await bookingService.ChangeBookingStatus(
            request.BookingId,
            BookingStatus.Canceled, new OfferDto
            {
                Id = request.OfferId
            });
        Console.WriteLine("Cancelled booking for " + request.BookingId);
    }
}