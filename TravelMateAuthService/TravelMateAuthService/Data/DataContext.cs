using Microsoft.EntityFrameworkCore;
using TravelMateAuthService.Entities;

namespace TravelMateAuthService.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
           : base(options) { }

        public DbSet<UserCredentials> Users { get; set; }
    }
}
