using System;
using MongoDB.Bson;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Resolute.ChatHub.IO
{
    public class RTMHub : Hub
    {
        // Bot Allocator Says
        public void RegisterBotFactory()
        {
            Console.WriteLine("Inside BotFactory");
            BotFactoryManager.Factories.Add(Context.ConnectionId);
            Clients.Client(Context.ConnectionId).SendAsync("Acknowledgement", "Done");
        }

        // Bot Says ..
        public async void AssignMeToUser(string groupId)
        {
            Console.WriteLine("Allocating To User");
            GroupHandler.UserGroupMapper.Add(Context.ConnectionId, groupId);
            Console.WriteLine($"Bot Trying to connect to {groupId}");
            await Groups.AddToGroupAsync(Context.ConnectionId, groupId);
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            if (BotFactoryManager.Factories.Contains(Context.ConnectionId))
            {
                Console.WriteLine("Bot Factory Closing");
                BotFactoryManager.Factories.Remove(Context.ConnectionId);
            }

            return base.OnDisconnectedAsync(exception);
        }

        // User Says...
        public void AllocateMeAnAgent(string query)
        {
            Console.WriteLine(query);
            var groupId = ObjectId.GenerateNewId().ToString();
            Groups.AddToGroupAsync(Context.ConnectionId, groupId);
            GroupHandler.UserGroupMapper.Add(Context.ConnectionId, groupId);
            Console.WriteLine($"Bot is connecting to {groupId}");
            var BotFactory = BotFactoryManager.Factory;
            if (!string.IsNullOrEmpty(BotFactory))
            {
                Clients.Client(BotFactory).SendAsync("AllocateMeABot", groupId, query);
            }
        }

        // User, Agent and Bot says Send Message
        public async void SendMessage(string message)
        {
            var groupId = GroupHandler.UserGroupMapper[Context.ConnectionId];
            await Clients.GroupExcept(groupId, Context.ConnectionId).SendAsync("message", message);
        }
    }
}
