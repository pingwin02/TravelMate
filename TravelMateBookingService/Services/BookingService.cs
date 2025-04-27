using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Runtime;
using TravelMateBookingService.Models.Bookings;
using TravelMateBookingService.Models.Bookings.DTO;
using TravelMate.Models.Messages;
using TravelMateBookingService.Models.Settings;
using TravelMateBookingService.Repositories;

namespace TravelMateBookingService.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IOptions<BookingsSettings> _settings;
        private readonly IRequestClient<CheckSeatAvailabilityRequest> _seatAvailabilityClient;


        public BookingService(IBookingRepository bookingRepository, IOptions<BookingsSettings> settings, IRequestClient<CheckSeatAvailabilityRequest> seatAvailabilityRequest)
        {
            _bookingRepository = bookingRepository;
            _settings = settings;
            _seatAvailabilityClient = seatAvailabilityRequest;
        }

        public async Task<Booking> CreateBooking(BookingRequestDto bookingRequestDto)
        {
            var isSeatAvailableResponse = await _seatAvailabilityClient.GetResponse<CheckSeatAvailabilityResponse>(
            new CheckSeatAvailabilityRequest
            {
                OfferId = bookingRequestDto.OfferId,
                SeatType = bookingRequestDto.SeatType
            });
            Console.WriteLine("Received seat availability response: " + isSeatAvailableResponse.Message.IsAvailable);
            if (!isSeatAvailableResponse.Message.IsAvailable) return null;

            var booking = new Booking
            {
                UserId = bookingRequestDto.UserId,
                OfferId = bookingRequestDto.OfferId,
                Status = BookingStatus.Pending,
                SeatNumber = bookingRequestDto.SeatNumber,
                SeatType = bookingRequestDto.SeatType,
                PassengerName = bookingRequestDto.PassengerName,
                PassengerType = bookingRequestDto.PassengerType,
                CreatedAt = DateTime.Now,
                ReservedUntil = DateTime.Now.AddSeconds(_settings.Value.BookingExpirationTime)
            };
            var savedBooking = await _bookingRepository.CreateBooking(booking);
            BookingExpirationService.AddBookingCancelationToQueue(savedBooking);
            return booking;
        }


        public async Task<Booking> GetBookingById(Guid bookingId)
        {
            return await _bookingRepository.GetBookingById(bookingId);
        }

        public async Task<List<Booking>> GetBookingsByUserId(Guid userId)
        {
            return await _bookingRepository.GetBookingsByUserId(userId);
        }



    }
}
    
