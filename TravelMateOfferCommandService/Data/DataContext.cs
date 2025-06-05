using Microsoft.EntityFrameworkCore;
using TravelMateOfferCommandService.Models;

namespace TravelMateOfferCommandService.Data;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
    public DbSet<Airline> Airlines { get; set; }
    public DbSet<Airplane> Airplanes { get; set; }
    public DbSet<Airport> Airports { get; set; }
    public DbSet<Offer> Offers { get; set; }
}