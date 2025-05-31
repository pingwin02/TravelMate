
using TravelMate.Models.Messages;
using TravelMateBookingService.Models.Bookings;
using TravelMateBookingService.Models.Bookings.DTO;

namespace TravelMateBookingService.Services;

public interface IBookingService
{
    Task<BookingDto> CreateBooking(Guid userId, BookingRequestDto bookingRequestDto);
    Task<Booking> GetBookingById(Guid userId, Guid bookingId);
    Task<List<Booking>> GetBookingsByUserId(Guid userId);
    Task<bool> ChangeBookingStatus(Guid bookingId, BookingStatus status);
    Task<bool> CheckIfPending(Guid bookingId);
}