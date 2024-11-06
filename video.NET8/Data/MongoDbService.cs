using MongoDB.Driver;
using Microsoft.Extensions.Configuration;
using LibraryManagementSystem.Domain;

namespace LibraryManagementSystem.Data
{
    public class MongoDbService
    {
        private readonly IMongoDatabase _database;

        // Constructor to initialize the MongoDB client and database
        public MongoDbService(IConfiguration configuration)
        {
            // Get MongoDB connection string from appsettings.json
            var connectionString = configuration.GetConnectionString("MongoDbConnection");
            var mongoUrl = MongoUrl.Create(connectionString);

            // Create a MongoClient with the connection string and access the database
            var mongoClient = new MongoClient(mongoUrl);
            _database = mongoClient.GetDatabase(mongoUrl.DatabaseName);
        }

        // Collection for BookMarketingInfo
        public IMongoCollection<BookMarketingInfo> BookMarketingInfo =>
            _database.GetCollection<BookMarketingInfo>("BookMarketingInfo");
    }
}
