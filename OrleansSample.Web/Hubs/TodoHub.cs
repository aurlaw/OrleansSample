using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Streams;
using OrleansSample.Interfaces;
using OrleansSample.Interfaces.Models;
using System.Linq;
using OrleansSample.Web.Services;

namespace OrleansSample.Web.Hubs
{
    public class TodoHub : Hub
    {
        private ILogger logger;
        private ILoggerFactory loggerFactory;
        private readonly IClusterClient orleansClient;

        private readonly ISubscriptionManager subscriptionManager;
        // private readonly ITodo todoGrain;
        public TodoHub(ILoggerFactory loggerFactory, IClusterClient orleansClient, ISubscriptionManager subscriptionManager)
        {
            this.orleansClient = orleansClient;
            this.loggerFactory = loggerFactory;
            this.logger = loggerFactory.CreateLogger($"{this.GetType().Name}");
            this.subscriptionManager = subscriptionManager;

        }
        public async Task AddTodo(string name)
        {
            var todo = new TodoItem(Guid.NewGuid(), name, DateTime.UtcNow);
            var todoGrain = orleansClient.GetGrain<ITodo>(Constants.TodoKey);
            await todoGrain.SetAsync(todo);
            await Clients.All.SendAsync("TodoAdded", todo);
        }

        public async Task ClearTodos() 
        {
            var todoGrain = orleansClient.GetGrain<ITodo>(Constants.TodoKey);
            await todoGrain.ClearAsync();
            await Clients.All.SendAsync("TodosCleared");
        }

        public async Task Subscribe(string subscriptionId)
        {
            Guid subId = Guid.Empty;
            Guid.TryParse(subscriptionId, out subId);

            var todoGrain = orleansClient.GetGrain<ITodo>(Constants.TodoKey);
            var result = await todoGrain.GetAllAsync();
            // create subscription
            var handleId = await subscriptionManager.CreateSubscription(Context, subId,  async(args) => {
                await args.SubscriptionGroup.SendAsync("Notification", args.Message);
            });
            // send message to caller
            await Clients.Caller.SendAsync("SubscribeReceived", new {result, handleId});
            logger.LogInformation("SubscribeReceived sent");
        }
    }
}