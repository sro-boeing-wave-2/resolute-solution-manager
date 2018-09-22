using System.Threading.Tasks;
using MongoDB.Bson;
using System;
using System.Collections.Generic;

namespace Resolute.ChatHub.Services
{
    public interface ISolutionService
    {
         String GetSolutionsByIntentAsync(string intent);
         Task CreateSolution(BsonDocument solution);
    }
}
