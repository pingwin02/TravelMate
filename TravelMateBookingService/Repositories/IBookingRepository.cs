using TravelMate.Messages.Models.Preferences;
using TravelMate.Models.Messages;
using TravelMate.Models.Offers;
using TravelMateBookingService.Models.Bookings;
using TravelMateBookingService.Models.Bookings.DTO;

namespace TravelMateBookingService.Repositories;

public interface IBookingRepository
{
    Task<Booking> CreateBooking(Booking booking);
    Task<bool> ChangeBookingStatus(Guid bookingId, BookingStatus status, OfferDto offer);
    Task<bool> CheckIfPending(Guid bookingId);
    Task<Booking> GetBookingById(Guid userId, Guid bookingId);
    Task<List<Booking>> GetBookingsByUserId(Guid userId);
    Task<IEnumerable<DeparturePreferenceDto>> GetDeparturePreferences();
    Task<OfferPreferencesSummaryDto> GetOfferPreferences();
}