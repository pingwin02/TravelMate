using MongoDB.Driver;
using TravelMate.Models.Offers;

public class DataContext
{
    private readonly IMongoDatabase _database;

    public DataContext()
    {
        var connectionString = "mongodb://admin:password@mongodb-container:27017";
        var client = new MongoClient(connectionString);
        _database = client.GetDatabase("TravelMateOfferQueryDatabase");
    }


    public IMongoDatabase Database => _database;
    public IMongoCollection<OfferDto> Offers => _database.GetCollection<OfferDto>("Offers");
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