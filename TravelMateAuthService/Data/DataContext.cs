using Microsoft.EntityFrameworkCore;
using TravelMateAuthService.Entities;

namespace TravelMateAuthService.Data
{
    public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
    {
        public DbSet<UserCredentials> Users { get; set; }
    }
}