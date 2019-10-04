using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Streams;
using OrleansSample.Interfaces;
using OrleansSample.Interfaces.Models;
using System.Linq;
namespace OrleansSample.Web.Hubs
{
    public class TodoHub : Hub
    {
        private ILogger logger;
        private ILoggerFactory loggerFactory;
        private readonly IClusterClient orleansClient;
        // private readonly ITodo todoGrain;
        public TodoHub(ILoggerFactory loggerFactory, IClusterClient orleansClient)
        {
            this.orleansClient = orleansClient;
            this.loggerFactory = loggerFactory;
            this.logger = loggerFactory.CreateLogger($"{this.GetType().Name}");

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
            if(subId != Guid.Empty) 
            {
                var subs = await orleansClient.GetStreamProvider(Constants.StreamProvider)
                    .GetStream<TodoNotification>(Constants.TodoKey, nameof(ITodo)).GetAllSubscriptionHandles();

                var existingSub = subs.FirstOrDefault(s => s.HandleId == subId);
                if(existingSub != null) {
                await existingSub.UnsubscribeAsync();
                }
            }    
            var todoGrain = orleansClient.GetGrain<ITodo>(Constants.TodoKey);
            var result = await todoGrain.GetAllAsync();
            var subscription = await orleansClient.GetStreamProvider(Constants.StreamProvider)
                .GetStream<TodoNotification>(Constants.TodoKey, nameof(ITodo))
                .SubscribeAsync(new TodoItemObserver(loggerFactory, Clients, async (notification, hubClient) => {
                    logger.LogInformation("Received Notification");
                    var record = string.Empty;
                    switch(notification.NotificationType)
                    {
                        case NotificationType.Add:
                            logger.LogInformation($"TodoHub Added: {notification.Item.Title}");
                            record = $"Added: {notification.Item.Title}";
                        break;
                        case NotificationType.Remove:
                            logger.LogInformation($"TodoHub Removed: {notification.Item.Title}");
                            record = $"Removed: {notification.Item.Title}";
                        break;
                        case NotificationType.Clear:
                            logger.LogInformation("TodoHub Cleared all Todos");
                            record = "Cleared all Todos";
                        break;
                    }
                    await hubClient.All.SendAsync("Notification", record);
                    logger.LogInformation("Notification Sent");
                    // return Task.CompletedTask;
                }));
            await Clients.All.SendAsync("SubscribeReceived", new {result, subscription.HandleId});
            logger.LogInformation("SubscribeReceived sent");

        }
    }
    public class TodoItemObserver : IAsyncObserver<TodoNotification>
    {
         private ILogger logger;
         private IHubCallerClients hubClient;
        private readonly Func<TodoNotification, IHubCallerClients, Task> action;

        public TodoItemObserver(ILoggerFactory loggerFactory, IHubCallerClients hubClient, Func<TodoNotification, IHubCallerClients, Task> action)
        {
            this.logger = loggerFactory.CreateLogger($"{this.GetType().Name}");
            this.hubClient = hubClient;
            this.action = action;
        }
        public Task OnCompletedAsync() => Task.CompletedTask;

        public Task OnErrorAsync(Exception ex)
        {
            logger.LogError(ex, ex.Message);
            return Task.CompletedTask;
        }

        public Task OnNextAsync(TodoNotification item, StreamSequenceToken token = null) => action(item, hubClient);
    }
}