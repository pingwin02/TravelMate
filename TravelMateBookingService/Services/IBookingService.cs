using TravelMateBookingService.Models.Bookings;
using TravelMateBookingService.Models.Bookings.DTO;

namespace TravelMateBookingService.Services;

public interface IBookingService
{
    Task<BookingDto> CreateBooking(Guid userId, BookingRequestDto bookingRequestDto);
    Task<Booking> GetBookingById(Guid userId, Guid bookingId);
    Task<List<Booking>> GetBookingsByUserId(Guid userId);
}