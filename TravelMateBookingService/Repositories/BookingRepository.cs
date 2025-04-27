using Microsoft.EntityFrameworkCore;
using TravelMateBackend.Data;
using TravelMateBookingService.Models.Bookings;

namespace TravelMateBookingService.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly DataContext _context;
        public BookingRepository(DataContext dataContext) {
            _context = dataContext;
        }

        public async Task<Booking> CreateBooking(Booking booking)
        {
            await _context.Bookings.AddAsync(booking);
            await _context.SaveChangesAsync();
            return booking;
        }

        public async Task<bool> ChangeBookingStatus(Guid bookingId, BookingStatus status)
        {
            var booking = await _context.Bookings.FindAsync(bookingId);
            //zamienic na logger potem
            Console.WriteLine($"Booking found: {booking != null}");
            if (booking == null) return false;
            booking.Status = status;
            
            _context.Bookings.Update(booking);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Booking> GetBookingById(Guid bookingId)
        {
            return await _context.Bookings.FindAsync(bookingId);
        }

        public async Task<List<Booking>> GetBookingsByUserId(Guid userId)
        {
            return await _context.Bookings
                .Where(b => b.UserId == userId)
                .ToListAsync();
        }
    }
}
