using TravelMateBookingService.Models.Bookings;
using TravelMateBookingService.Models.Bookings.DTO;

namespace TravelMateBookingService.Services
{
    public interface IBookingService
    {
        Task<Booking> CreateBooking(BookingRequestDto newBooking);
        Task<Booking> GetBookingById(Guid bookingId);
        Task<List<Booking>> GetBookingsByUserId(Guid userId);
    }
}
