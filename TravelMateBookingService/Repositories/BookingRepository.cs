using Microsoft.EntityFrameworkCore;
using TravelMateBackend.Data;
using TravelMateBookingService.Models.Bookings;

namespace TravelMateBookingService.Repositories;

public class BookingRepository(DataContext dataContext) : IBookingRepository
{
    public async Task<Booking> CreateBooking(Booking booking)
    {
        await dataContext.Bookings.AddAsync(booking);
        await dataContext.SaveChangesAsync();
        return booking;
    }

    public async Task<bool> ChangeBookingStatus(Guid bookingId, BookingStatus status)
    {
        var booking = await dataContext.Bookings.FindAsync(bookingId);
        //zamienic na logger potem
        Console.WriteLine($"Booking found: {booking != null}");
        if (booking == null) return false;
        booking.Status = status;

        dataContext.Bookings.Update(booking);
        await dataContext.SaveChangesAsync();
        return true;
    }

    public async Task<Booking> GetBookingById(Guid bookingId)
    {
        var booking = await dataContext.Bookings.FindAsync(bookingId);

        if (booking == null)
            throw new KeyNotFoundException($"Booking with id {bookingId} not found");

        return booking;
    }

    public async Task<List<Booking>> GetBookingsByUserId(Guid userId)
    {
        return await dataContext.Bookings
            .Where(b => b.UserId == userId)
            .ToListAsync();
    }
}