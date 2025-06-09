using TravelMate.Models.Messages;
using TravelMate.Models.Offers;
using TravelMateBookingService.Models.Bookings;
using TravelMateBookingService.Models.Bookings.DTO;

namespace TravelMateBookingService.Services;

public interface IBookingService
{
    Task<BookingDto> CreateBooking(Guid userId, BookingRequestDto bookingRequestDto);
    Task<Booking> GetBookingById(Guid userId, Guid bookingId);
    Task<List<Booking>> GetBookingsByUserId(Guid userId);
    Task<bool> ChangeBookingStatus(Guid bookingId, BookingStatus status, OfferDto offer);
    Task<bool> CheckIfPending(Guid bookingId);
    Task<IEnumerable<DeparturePreferenceDto>> GetDeparturePreferences();
    Task<IEnumerable<OfferPreferencesDto>> GetOfferPreferences(Guid offerId);
}