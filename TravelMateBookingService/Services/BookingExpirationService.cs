using System.Collections.Concurrent;
using MassTransit;
using TravelMate.Models.Messages;
using TravelMateBookingService.Models.Bookings;
using TravelMateBookingService.Repositories;

namespace TravelMateBookingService.Services;

public class BookingExpirationService(IServiceProvider serviceProvider) : BackgroundService
{
    private static readonly ConcurrentQueue<(Guid BookingId, SeatType seatType, Guid OfferId, DateTime CancelAt)>
        Queue = new();

    public static void AddBookingCancellationToQueue(Booking booking)
    {
        Console.WriteLine($"Adding booking {booking.Id} to queue for cancellation at {booking.ReservedUntil}");
        Queue.Enqueue((booking.Id, booking.SeatType, booking.OfferId, booking.ReservedUntil.Value));
    }

    public async Task CancelBooking(Guid bookingId, SeatType seatType, Guid offerId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var scope = serviceProvider.CreateScope();
            var bookingRepository = scope.ServiceProvider.GetRequiredService<IBookingRepository>();

            if (await bookingRepository.CheckIfCancelled(bookingId))
            {
                Console.WriteLine($"Booking {bookingId} is already canceled");
                return;
            }

            var cancelReservationClient = scope.ServiceProvider
                .GetRequiredService<IRequestClient<CancelReservationRequest>>();

            var response = await cancelReservationClient.GetResponse<CancelReservationResponse>(
                new CancelReservationRequest
                {
                    OfferId = offerId,
                    SeatType = seatType
                }, cancellationToken);

            if (response.Message.IsCanceled)
            {
                Console.WriteLine($"Seat is available again, canceling {bookingId}");
                await bookingRepository.ChangeBookingStatus(bookingId, BookingStatus.Canceled);
            }

            Console.WriteLine($"Failed to cancel booking {bookingId}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during CancelBooking: {ex.Message}");
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
            if (Queue.TryDequeue(out var task))
            {
                var delay = task.CancelAt - DateTime.Now;
                if (delay > TimeSpan.Zero)
                    await Task.Delay(delay, stoppingToken);

                await CancelBooking(task.BookingId, task.seatType, task.OfferId, stoppingToken);
            }
            else
            {
                await Task.Delay(100, stoppingToken);
            }
    }
}