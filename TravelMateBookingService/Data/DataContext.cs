using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using TravelMate.Models.Messages;
using TravelMateBookingService.Models.Bookings;

namespace TravelMateBookingService.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options, IConfiguration configuration) : base(options)
    {
        var connectionString = configuration["MongoDB:ConnectionString"];
        var databaseName = configuration["MongoDB:DatabaseName"];

        var mongoClient = new MongoClient(connectionString);
        mongoDatabase = mongoClient.GetDatabase(databaseName);
    }

    public DbSet<Booking> Bookings { get; set; }

    public IMongoDatabase mongoDatabase { get; }

    public IMongoCollection<BookingEvent> BookingEvents => mongoDatabase.GetCollection<BookingEvent>("Bookings");
}

public class MongoDbInitializer
{
    public static async Task InitializeMongoDbAsync(DataContext context)
    {
        var collectionNames = await context.mongoDatabase.ListCollectionNamesAsync();
        var collections = await collectionNames.ToListAsync();


        if (!collections.Contains("Bookings"))
        {
            await context.mongoDatabase.CreateCollectionAsync("Bookings");
            Console.WriteLine("Created 'Bookings' collection.");
        }
    }
}