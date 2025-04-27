using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Runtime;
using TravelMateBookingService.Models.Bookings;
using TravelMateBookingService.Models.Bookings.DTO;
using TravelMateBookingService.Models.Settings;
using TravelMateBookingService.Repositories;

namespace TravelMateBookingService.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IOptions<BookingsSettings> _settings;


        public BookingService(IBookingRepository bookingRepository, IOptions<BookingsSettings> settings)
        {
            _bookingRepository = bookingRepository;
            _settings = settings;
        }

        public async Task<Booking> CreateBooking(BookingRequestDto bookingRequestDto)
        {
            var booking = new Booking
            {
                UserId = bookingRequestDto.UserId,
                OfferId = bookingRequestDto.OfferId,
                Status = BookingStatus.Pending,
                SeatNumber = bookingRequestDto.SeatNumber,
                SeatType = bookingRequestDto.SeatType,
                PassengerName = bookingRequestDto.PassengerName,
                PassengerType = bookingRequestDto.PassengerType,
                CreatedAt = DateTime.UtcNow,
                ReservedUntil = DateTime.UtcNow.AddSeconds(_settings.Value.BookingExpirationTime)
            };
            var savedBooking = await _bookingRepository.CreateBooking(booking);
            BookingExpirationService.AddBookingCancelationToQueue(savedBooking.Id, savedBooking.ReservedUntil.Value);
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
    
