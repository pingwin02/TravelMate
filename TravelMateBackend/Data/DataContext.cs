using Microsoft.EntityFrameworkCore;
using Models.Entities.Offers;
using TravelMateBackend.Models.Entities;

namespace TravelMateBackend.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Airline> Airlines { get; set; }    
        public DbSet<Airplane> Airplanes { get; set; }
        public DbSet<Airport> Airports { get; set; }
        public DbSet<Offer> Offers { get; set; }


    }
}
