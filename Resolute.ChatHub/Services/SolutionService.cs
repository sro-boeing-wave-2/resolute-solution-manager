using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core;
using Resolute.ChatHub.Data;
using Resolute.ChatHub.Models;
using System.Threading.Tasks;

namespace Resolute.ChatHub.Services
{
    public class SolutionService: ISolutionService
    {
        public SolutionDbContext _context;
        public SolutionService(IOptions<Settings> settings)
        {
            _context = new SolutionDbContext(settings);
        }

        public String GetSolutionsByIntentAsync(string intent)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("Intent", intent);
            var documents = _context.Solutions.Find(filter).ToList();
            return documents.ToJson();
        }

        public async Task CreateSolution(BsonDocument solution)
        {
            await _context.Solutions.InsertOneAsync(solution);
        }
    }
}
