using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using TravelMate.Models.Messages;
using TravelMate.Models.Offers;
using TravelMateBookingService.Hubs;
using TravelMateBookingService.Models.Bookings;
using TravelMateBookingService.Models.Bookings.DTO;
using TravelMateBookingService.Models.Settings;
using TravelMateBookingService.Repositories;

namespace TravelMateBookingService.Services;

public class BookingService(
    IBookingRepository bookingRepository,
    IOptions<BookingsSettings> settings,
    IRequestClient<BookingStartedEvent> bookingRequestClient,
    IBus bus,
    IHubContext<DeparturePreferencesHub> departurePreferencesHub,
    IHubContext<OfferPreferencesHub> offerPreferencesHub)
    : IBookingService
{
    public async Task<BookingDto> CreateBooking(Guid userId, BookingRequestDto bookingRequestDto)
    {
        var bookingId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();
        var response = await bookingRequestClient.GetResponse<BookingSagaStatusResponse>(
            new BookingStartedEvent
            {
                CorrelationId = correlationId,
                OfferId = bookingRequestDto.OfferId,
                BookingId = bookingId,
                SeatType = bookingRequestDto.SeatType,
                PassengerType = bookingRequestDto.PassengerType
            });

        var result = response.Message;
        Console.WriteLine($"Received payment result for {result.CorrelationId} {result.IsSuccessful}");

        if (!result.IsSuccessful)
            throw new InvalidOperationException(
                $"Booking id {bookingId} failed. No available seats for offer {bookingRequestDto.OfferId}");

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
        BookingExpirationService.AddBookingCancellationToQueue(savedBooking, correlationId);
        return new BookingDto
        {
            Id = savedBooking.Id,
            CreatedAt = savedBooking.CreatedAt,
            ReservedUntil = savedBooking.ReservedUntil,
            PaymentId = savedBooking.PaymentId.Value
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

    public async Task<bool> ChangeBookingStatus(Guid bookingId, BookingStatus status, OfferDto offer)
    {
        var result = await bookingRepository.ChangeBookingStatus(bookingId, status, offer);

        if (result && status == BookingStatus.Confirmed)
        {
            var preferences = bookingRepository.GetDeparturePreferences();
            await departurePreferencesHub.Clients.All.SendAsync("ReceivePreferencesUpdate", preferences);

            var offerPreferences = bookingRepository.GetOfferPreferences(offer.Id);
            await offerPreferencesHub.Clients.Group(offer.Id.ToString())
                .SendAsync("ReceiveOfferPreferencesUpdate", offerPreferences);
        }
        return result;
    }

    public Task<bool> CheckIfPending(Guid bookingId)
    {
        return bookingRepository.CheckIfPending(bookingId);
    }

    public async Task<IEnumerable<DeparturePreferenceDto>> GetDeparturePreferences()
    {
        var preferences = await bookingRepository.GetDeparturePreferences();
        if (preferences == null)
        {
            throw new InvalidOperationException("Departure preferences not found.");
        }
        return preferences;
    }

    public Task<IEnumerable<OfferPreferencesDto>> GetOfferPreferences(Guid offerId)
    {
        var preferences = bookingRepository.GetOfferPreferences(offerId);
        if (preferences == null)
        {
            throw new InvalidOperationException("Offer preferences not found.");
        }
        return preferences;
    }
}