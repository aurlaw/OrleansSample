using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Streams;
using OrleansSample.Interfaces;
using OrleansSample.Interfaces.Models;
using OrleansSample.Web.Hubs;
using System.Linq;

namespace OrleansSample.Web.Services
{
    public class SubscriptionManager : ISubscriptionManager
    {
        public const string SubGroup = "TodoSubGroup";

        private ILogger logger;
        private ILoggerFactory loggerFactory;
        private readonly IClusterClient orleansClient;

        private bool isSubsriptionEnabled;
        private StreamSubscriptionHandle<TodoNotification> subscription;
        private readonly IHubContext<TodoHub> hubContext;
        public SubscriptionManager(ILoggerFactory loggerFactory, IClusterClient orleansClient, IHubContext<TodoHub> hubContext)
        {
            this.orleansClient = orleansClient;
            this.loggerFactory = loggerFactory;
            this.logger = loggerFactory.CreateLogger($"{this.GetType().Name}");
            this.hubContext = hubContext;
            
        }

        public async Task<Guid> CreateSubscription(HubCallerContext context, Guid subscriptionId, Action<SubscriptionEventArgs> action)
        {
            if(subscriptionId != Guid.Empty) 
            {
                var subs = await orleansClient.GetStreamProvider(Constants.StreamProvider)
                    .GetStream<TodoNotification>(Constants.TodoKey, nameof(ITodo)).GetAllSubscriptionHandles();

                var existingSub = subs.FirstOrDefault(s => s.HandleId == subscriptionId);
                if(existingSub != null) {
                    // remove from group
                    await hubContext.Groups.RemoveFromGroupAsync(context.ConnectionId, SubGroup);
                }
            }    
            // only create one subscription - 
            if(!isSubsriptionEnabled) 
            {
                this.subscription = await orleansClient.GetStreamProvider(Constants.StreamProvider)
                    .GetStream<TodoNotification>(Constants.TodoKey, nameof(ITodo))
                    .SubscribeAsync(new TodoItemObserver(loggerFactory, (notification) => {
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
                        action(new SubscriptionEventArgs(hubContext, record, notification.Item));
                        logger.LogInformation("Notification Sent");
                        return Task.CompletedTask;
                    }));    
                isSubsriptionEnabled = true;   
                
            }
            // add user to group    
            await  hubContext.Groups.AddToGroupAsync(context.ConnectionId, SubGroup);    
            return this.subscription.HandleId;
        }
    }

    public class SubscriptionEventArgs : EventArgs 
    {
        public SubscriptionEventArgs(IHubContext<TodoHub> hubContext, string message, TodoItem item)
        {
            this.HubContext = hubContext;
            this.SubscriptionGroup = hubContext.Clients.Group(SubscriptionManager.SubGroup);
            this.Message = message;
            this.Item = item;
        }
        public string Message {get;}
        public TodoItem Item {get;}
        public IHubContext<TodoHub> HubContext {get;}

        public IClientProxy SubscriptionGroup {get;}
    }
}