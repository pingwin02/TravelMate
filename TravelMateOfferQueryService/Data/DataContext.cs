using MongoDB.Driver;
using TravelMate.Models.Offers;

public class DataContext
{
    public DataContext(IConfiguration configuration)
    {
        var connectionString = configuration["MongoDB:ConnectionString"];
        var databaseName = configuration["MongoDB:DatabaseName"];

        var client = new MongoClient(connectionString);
        Database = client.GetDatabase(databaseName);
    }


    public IMongoDatabase Database { get; }

    public IMongoCollection<OfferDto> Offers => Database.GetCollection<OfferDto>("Offers");
}

public class MongoDbInitializer
{
    public static async Task InitializeMongoDbAsync(DataContext context)
    {
        var collectionNames = await context.Database.ListCollectionNamesAsync();
        var collections = await collectionNames.ToListAsync();


        if (!collections.Contains("Offers"))
        {
            await context.Database.CreateCollectionAsync("Offers");
            Console.WriteLine("Created 'Offers' collection.");
        }
    }
}