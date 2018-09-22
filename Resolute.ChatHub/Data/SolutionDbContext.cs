using System;
using MongoDB.Bson;
using MongoDB.Driver;
using Resolute.ChatHub.Models;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;

namespace Resolute.ChatHub.Data
{
    public class SolutionDbContext
    {
        private readonly IMongoDatabase _database = null;

        public SolutionDbContext(IOptions<Settings> settings)
        {
            Console.WriteLine(settings.Value.ConnectionString);
            var client = new MongoClient(settings.Value.ConnectionString);
            if (client!=null)
                _database = client.GetDatabase(settings.Value.Database);
        }

        public IMongoCollection<BsonDocument> Solutions
        {
            get
            {
                return _database.GetCollection<BsonDocument>("templates");
            }
        }
    }
}
