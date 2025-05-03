using Microsoft.EntityFrameworkCore;
using TravelMatePaymentService.Models.Bookings;

namespace TravelMatePaymentService.Data;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
    public DbSet<Payment> Payments { get; set; }
}