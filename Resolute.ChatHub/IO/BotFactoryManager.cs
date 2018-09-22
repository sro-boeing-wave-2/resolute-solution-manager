using System;
using System.Collections.Generic;
using System.Linq;

namespace Resolute.ChatHub.IO
{
    public static class BotFactoryManager
    {
        public static List<String> Factories { get; set; } = new List<string>();

        public static string Factory {
            get
            {
                if (BotFactoryManager.Factories.Count > 0)
                {
                    var factoryToBeReturned = BotFactoryManager.Factories.Take(1).First();
                    // Rotating the list to load balance the list of Factories
                    BotFactoryManager.Factories = BotFactoryManager.Factories.Skip(1).Concat(BotFactoryManager.Factories.Take(1)).ToList();
                    return factoryToBeReturned;
                }
                return "";
            }
        }
    }
}
