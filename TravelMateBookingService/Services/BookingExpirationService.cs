using MassTransit;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using TravelMate.Models.Messages;
using TravelMateBookingService.Models.Bookings;
using TravelMateBookingService.Models.Bookings.DTO;
using TravelMateBookingService.Models.Settings;
using TravelMateBookingService.Repositories;

namespace TravelMateBookingService.Services
{
    public class BookingExpirationService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        private static readonly ConcurrentQueue<(Guid BookingId, SeatType seatType, Guid OfferId, DateTime CancelAt)> _queue = new();

        public BookingExpirationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;


        }

        public static void AddBookingCancelationToQueue(Booking booking )
        {
            Console.WriteLine($"Adding booking {booking.Id} to queue for cancellation at {booking.ReservedUntil}");
            _queue.Enqueue((booking.Id,booking.SeatType,booking.OfferId, booking.ReservedUntil.Value));
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_queue.TryDequeue(out var task))
                {
                    var delay = task.CancelAt - DateTime.Now;

                    if (delay > TimeSpan.Zero)
                        await Task.Delay(delay, stoppingToken);

                    try
                    {
                        using (var scope = _serviceProvider.CreateScope())
                        {
                            var bookingRepostiory = scope.ServiceProvider.GetRequiredService<IBookingRepository>();
                            var cancelReservationClient = scope.ServiceProvider.GetRequiredService<IRequestClient<CancelReservationRequest>>();
                            var response = await cancelReservationClient.GetResponse<CancelReservationResponse>(
                            new CancelReservationRequest
                            {
                                OfferId = task.OfferId,
                                SeatType = task.seatType
                            });
                            if(response.Message.IsCanceled)
                            {
                                Console.WriteLine($"Seat is available again, canceling {task.BookingId}");
                                await bookingRepostiory.ChangeBookingStatus(task.BookingId, BookingStatus.Canceled);
                            }
                            else
                            {
                                Console.WriteLine($"Failed to cancel booking {task.BookingId}");
                            }
                        }

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

