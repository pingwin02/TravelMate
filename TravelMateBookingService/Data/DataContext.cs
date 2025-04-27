using Microsoft.EntityFrameworkCore;
using TravelMateBookingService.Models.Bookings;


namespace TravelMateBackend.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Booking> Bookings { get; set; }
    }
}
