using Microsoft.EntityFrameworkCore;
using TravelMate.Models.Messages;
using TravelMateBookingService.Data;
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

        if (booking == null)
            throw new KeyNotFoundException($"Booking with id {bookingId} not found");

        booking.Status = status;

        dataContext.Bookings.Update(booking);
        await dataContext.SaveChangesAsync();
        return true;
    }

    public Task<bool> CheckIfPending(Guid bookingId)
    {
        var booking = dataContext.Bookings.Find(bookingId);
        return Task.FromResult(booking is { Status: BookingStatus.Pending });
    }

    public async Task<Booking> GetBookingById(Guid userId, Guid bookingId)
    {
        var booking = await dataContext.Bookings.FindAsync(bookingId);

        if (booking == null || booking.UserId != userId)
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