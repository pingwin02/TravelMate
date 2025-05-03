using TravelMateBookingService.Models.Bookings;

namespace TravelMateBookingService.Repositories;

public interface IBookingRepository
{
    Task<Booking> CreateBooking(Booking booking);
    Task<bool> ChangeBookingStatus(Guid bookingId, BookingStatus status);
    Task<Booking> GetBookingById(Guid bookingId);
    Task<List<Booking>> GetBookingsByUserId(Guid userId);
}