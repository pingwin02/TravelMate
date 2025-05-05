using MassTransit;
using MassTransit.Clients;
using Microsoft.Extensions.Options;
using System.Reflection.Metadata;
using TravelMate.Models.Messages;
using TravelMateBookingService.Models.Bookings;
using TravelMateBookingService.Models.Bookings.DTO;
using TravelMateBookingService.Models.Settings;
using TravelMateBookingService.Repositories;

namespace TravelMateBookingService.Services;

public class BookingService(
    IBookingRepository bookingRepository,
    IOptions<BookingsSettings> settings,
    IRequestClient<CheckSeatAvailabilityRequest> seatAvailabilityRequest,
    IRequestClient<PaymentCreationRequest> paymentRequest,
    IPublishEndpoint publishEndpoint,
    IBus bus)
    : IBookingService
{
    public async Task<BookingDto> CreateBooking(Guid userId, BookingRequestDto bookingRequestDto)
    {

        var correlationId = Guid.NewGuid();
        var bookingId = Guid.NewGuid();
        var task = new TaskCompletionSource<BookingSagaStatusResponse>();
        var handle = bus.ConnectReceiveEndpoint($"booking-status-response-{correlationId}", e =>
        {
            e.Handler<BookingSagaStatusResponse>(context =>
            {
                Console.WriteLine($"Received payment result for {context.Message.CorrelationId}");
                task.TrySetResult(context.Message);
                return Task.CompletedTask;
            });
        });
        await publishEndpoint.Publish(new BookingStartedEvent
        {
            CorrelationId = correlationId,
            OfferId = bookingRequestDto.OfferId,
            BookingId = bookingId,
            SeatType = bookingRequestDto.SeatType,
            PassengerType = bookingRequestDto.PassengerType
        });

       
        var result = await task.Task;
        Console.WriteLine($"Received payment result for {result.CorrelationId} {result.IsSuccessful}");

        if (!result.IsSuccessful)
            throw new InvalidOperationException();

        var booking = new Booking
        {
            Id = bookingId,
            UserId = userId,
            OfferId = bookingRequestDto.OfferId,
            Status = BookingStatus.Pending,
            SeatType = bookingRequestDto.SeatType,
            PassengerName = bookingRequestDto.PassengerName,
            PassengerType = bookingRequestDto.PassengerType,
            CreatedAt = DateTime.Now,
            ReservedUntil = DateTime.Now.AddSeconds(settings.Value.BookingExpirationTime),
            PaymentId = result.PaymentId,
            CorrelationId = correlationId
        };
       
        var savedBooking = await bookingRepository.CreateBooking(booking);
        Console.WriteLine(savedBooking);
        BookingExpirationService.AddBookingCancellationToQueue(savedBooking,correlationId);
        return new BookingDto
        {
            Id = savedBooking.Id,
            CreatedAt = savedBooking.CreatedAt,
            ReservedUntil = savedBooking.ReservedUntil,
            PaymentId = savedBooking.PaymentId.Value
        };

        //var response = await requestClient.GetResponse<BookingSagaStatusResponse>(new BookingStartedEvent
        //{
        //    CorrelationId = correlationId,
        //    OfferId = bookingRequestDto.OfferId,
        //    BookingId = bookingId,
        //    SeatType = bookingRequestDto.SeatType,
        //    PassengerType = bookingRequestDto.PassengerType
        //});

        //if (response.Message.IsSuccessful)
        //{
        //    
        //}
        //return null;



        //var isSeatAvailableResponse = await seatAvailabilityRequest.GetResponse<CheckSeatAvailabilityResponse>(
        //    new CheckSeatAvailabilityRequest
        //    {
        //        OfferId = bookingRequestDto.OfferId,
        //        SeatType = bookingRequestDto.SeatType,
        //        PassengerType = bookingRequestDto.PassengerType
        //    });
        //Console.WriteLine("Received seat availability response: " + isSeatAvailableResponse.Message.IsAvailable);
        //if (!isSeatAvailableResponse.Message.IsAvailable) throw new SeatNotAvailableException();

        //var booking = new Booking
        //{
        //    Id = Guid.NewGuid(),
        //    UserId = userId,
        //    OfferId = bookingRequestDto.OfferId,
        //    Status = BookingStatus.Pending,
        //    SeatType = bookingRequestDto.SeatType,
        //    PassengerName = bookingRequestDto.PassengerName,
        //    PassengerType = bookingRequestDto.PassengerType,
        //    CreatedAt = DateTime.Now,
        //    ReservedUntil = DateTime.Now.AddSeconds(settings.Value.BookingExpirationTime)
        //};

        //var paymentResponse = await paymentRequest.GetResponse<PaymentCreationResponse>(
        //    new PaymentCreationRequest
        //    {
        //        BookingId = booking.Id,
        //        Price = isSeatAvailableResponse.Message.DynamicPrice
        //    });

        //booking.PaymentId = paymentResponse.Message.PaymentId;

        //var savedBooking = await bookingRepository.CreateBooking(booking);
        //BookingExpirationService.AddBookingCancellationToQueue(savedBooking);

        //return new BookingDto
        //{
        //    Id = savedBooking.Id,
        //    CreatedAt = savedBooking.CreatedAt,
        //    ReservedUntil = savedBooking.ReservedUntil,
        //    PaymentId = savedBooking.PaymentId.Value
        //};
    }


    public async Task<Booking> GetBookingById(Guid userId, Guid bookingId)
    {
        return await bookingRepository.GetBookingById(userId, bookingId);
    }

    public async Task<List<Booking>> GetBookingsByUserId(Guid userId)
    {
        return await bookingRepository.GetBookingsByUserId(userId);
    }

    public async Task<bool> ChangeBookingStatus(Guid bookingId, BookingStatus status)
    {
        return await bookingRepository.ChangeBookingStatus(bookingId, status);
    }

    public Task<bool> CheckIfPending(Guid bookingId)
    {
        return bookingRepository.CheckIfPending(bookingId);
    }
}