using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using TravelMateBookingService.Models.Bookings;
using TravelMateBookingService.Models.Settings;
using TravelMateBookingService.Repositories;

namespace TravelMateBookingService.Services
{
    public class BookingExpirationService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private static readonly ConcurrentQueue<(Guid BookingId, DateTime CancelAt)> _queue = new();

        public BookingExpirationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

        }

        public static void AddBookingCancelationToQueue(Guid bookingId, DateTime cancelAt)
        {
            Console.WriteLine($"Adding booking {bookingId} to queue for cancellation at {cancelAt}");
            _queue.Enqueue((bookingId, cancelAt));
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_queue.TryDequeue(out var task))
                {
                    var delay = task.CancelAt - DateTime.UtcNow;

                    if (delay > TimeSpan.Zero)
                        await Task.Delay(delay, stoppingToken);

                    try
                    {
                        using var scope = _serviceProvider.CreateScope();
                        var bookingRepostiory = scope.ServiceProvider.GetRequiredService<IBookingRepository>();
                        await bookingRepostiory.ChangeBookingStatus(task.BookingId, BookingStatus.Canceled);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Failed to cancel booking: " + ex.Message);
                    }
                }
                else
                {
                    await Task.Delay(100, stoppingToken);
                }
            }
        }
    }
}

