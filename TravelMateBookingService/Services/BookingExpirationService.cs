using System.Collections.Concurrent;
using MassTransit;
using TravelMate.Models.Messages;
using TravelMateBookingService.Models.Bookings;
using TravelMateBookingService.Repositories;

namespace TravelMateBookingService.Services;

public class BookingExpirationService(IServiceProvider serviceProvider) : BackgroundService
{
    private static readonly ConcurrentQueue<(Guid BookingId, SeatType seatType, Guid OfferId, DateTime CancelAt, Guid
            correlationId)>
        Queue = new();

    public static void AddBookingCancellationToQueue(Booking booking, Guid correlationId)
    {
        Console.WriteLine($"Adding booking {booking.Id} to queue for cancellation at {booking.ReservedUntil}");
        Queue.Enqueue((booking.Id, booking.SeatType, booking.OfferId, booking.ReservedUntil, correlationId));
    }

    public async Task CancelBooking(Guid bookingId, SeatType seatType, Guid offerId, Guid correlationId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var scope = serviceProvider.CreateScope();
            var bookingRepository = scope.ServiceProvider.GetRequiredService<IBookingRepository>();
            var publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

            if (!await bookingRepository.CheckIfPending(bookingId))
            {
                Console.WriteLine($"Booking {bookingId} is not pending, skipping cancellation");
                return;
            }

            var res = await bookingRepository.ChangeBookingStatus(bookingId, BookingStatus.Canceled);

            await publishEndpoint.Publish(new BookingCancelledEvent
            {
                CorrelationId = correlationId,
                BookingId = bookingId
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during CancelBooking: {ex.Message}");
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine("BookingExpirationService started");
        while (!stoppingToken.IsCancellationRequested)
            if (Queue.TryDequeue(out var task))
            {
                var delay = task.CancelAt - DateTime.Now;
                if (delay > TimeSpan.Zero)
                    await Task.Delay(delay, stoppingToken);

                await CancelBooking(task.BookingId, task.seatType, task.OfferId, task.correlationId, stoppingToken);
            }
            else
            {
                await Task.Delay(100, stoppingToken);
            }
    }
}