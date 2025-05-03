using MassTransit;
using TravelMate.Models.Messages;
using TravelMateBookingService.Services;

namespace TravelMateBookingService.Consumers;

public class BookingStatusUpdateConsumer(IServiceProvider serviceProvider)
    : IConsumer<BookingStatusUpdateRequest>
{
    public async Task Consume(ConsumeContext<BookingStatusUpdateRequest> context)
    {
        var bookingStatusUpdateRequest = context.Message;
        var bookingId = bookingStatusUpdateRequest.BookingId;
        Console.WriteLine($"Received booking status update request for booking {bookingId}");
        var bookingService = serviceProvider.GetRequiredService<IBookingService>();

        var isUpdated = true;

        if (!await bookingService.CheckIfPending(bookingId))
        {
            Console.WriteLine($"Booking {bookingId} is not pending, skipping cancellation");
            isUpdated = false;
        }

        await bookingService.ChangeBookingStatus(
            bookingId,
            bookingStatusUpdateRequest.Status);

        Console.WriteLine($"Booking status updated for booking {bookingId}");
        await context.RespondAsync(new BookingStatusUpdateResponse
        {
            IsUpdated = isUpdated
        });
    }
}