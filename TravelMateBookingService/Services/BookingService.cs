using MassTransit;
using Microsoft.Extensions.Options;
using TravelMate.Models.Messages;
using TravelMateBookingService.Controllers.Exceptions;
using TravelMateBookingService.Models.Bookings;
using TravelMateBookingService.Models.Bookings.DTO;
using TravelMateBookingService.Models.Settings;
using TravelMateBookingService.Repositories;

namespace TravelMateBookingService.Services;

public class BookingService(
    IBookingRepository bookingRepository,
    IOptions<BookingsSettings> settings,
    IRequestClient<CheckSeatAvailabilityRequest> seatAvailabilityRequest,
    IRequestClient<PaymentRequest> paymentRequest)
    : IBookingService
{
    public async Task<BookingDto> CreateBooking(Guid userId, BookingRequestDto bookingRequestDto)
    {
        var isSeatAvailableResponse = await seatAvailabilityRequest.GetResponse<CheckSeatAvailabilityResponse>(
            new CheckSeatAvailabilityRequest
            {
                OfferId = bookingRequestDto.OfferId,
                SeatType = bookingRequestDto.SeatType,
                PassengerType = bookingRequestDto.PassengerType
            });
        Console.WriteLine("Received seat availability response: " + isSeatAvailableResponse.Message.IsAvailable);
        if (!isSeatAvailableResponse.Message.IsAvailable) throw new SeatNotAvailableException();

        var booking = new Booking
        {
            UserId = userId,
            OfferId = bookingRequestDto.OfferId,
            Status = BookingStatus.Pending,
            SeatNumber = bookingRequestDto.SeatNumber,
            SeatType = bookingRequestDto.SeatType,
            PassengerName = bookingRequestDto.PassengerName,
            PassengerType = bookingRequestDto.PassengerType,
            CreatedAt = DateTime.Now,
            ReservedUntil = DateTime.Now.AddSeconds(settings.Value.BookingExpirationTime)
        };
        var savedBooking = await bookingRepository.CreateBooking(booking);
        BookingExpirationService.AddBookingCancellationToQueue(savedBooking);

        var paymentResponse = await paymentRequest.GetResponse<PaymentResponse>(
            new PaymentRequest
            {
                BookingId = savedBooking.Id,
                Price = isSeatAvailableResponse.Message.DynamicPrice
            });

        return new BookingDto
        {
            Id = savedBooking.Id,
            CreatedAt = savedBooking.CreatedAt,
            ReservedUntil = savedBooking.ReservedUntil,
            PaymentId = paymentResponse.Message.PaymentId
        };
    }


    public async Task<Booking> GetBookingById(Guid userId, Guid bookingId)
    {
        return await bookingRepository.GetBookingById(userId, bookingId);
    }

    public async Task<List<Booking>> GetBookingsByUserId(Guid userId)
    {
        return await bookingRepository.GetBookingsByUserId(userId);
    }
}