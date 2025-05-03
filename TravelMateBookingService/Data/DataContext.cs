using Microsoft.EntityFrameworkCore;
using TravelMateBookingService.Models.Bookings;

namespace TravelMateBookingService.Data;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
    public DbSet<Booking> Bookings { get; set; }
}