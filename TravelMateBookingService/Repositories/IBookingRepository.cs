using TravelMate.Models.Messages;
using TravelMateBookingService.Models.Bookings;

namespace TravelMateBookingService.Repositories;

public interface IBookingRepository
{
    Task<Booking> CreateBooking(Booking booking);
    Task<bool> ChangeBookingStatus(Guid bookingId, BookingStatus status);
    Task<bool> CheckIfPending(Guid bookingId);
    Task<Booking> GetBookingById(Guid userId, Guid bookingId);
    Task<List<Booking>> GetBookingsByUserId(Guid userId);
}