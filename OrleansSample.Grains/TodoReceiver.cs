using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Streams;
using OrleansSample.Interfaces;
using OrleansSample.Interfaces.Models;

namespace OrleansSample.Grains
{
    [ImplicitStreamSubscription(nameof(ITodo))]
    public class TodoReceiver : Grain, ITodoReceiver, IGrainMarker
    {
        private ILogger logger;
        public TodoReceiver(ILoggerFactory loggerFactory)
        {
            this.logger = loggerFactory.CreateLogger($"{this.GetType().Name}-{this.IdentityString}");
        }
        public override async Task OnActivateAsync() 
        {
            // subscribe to TodoNotifications
            var stream = GetStreamProvider(Constants.StreamProvider)
            .GetStream<TodoNotification>(Constants.TodoKey, nameof(ITodo));

            await stream.SubscribeAsync((item, token) => {
                switch(item.NotificationType)
                {
                    case NotificationType.Add:
                        logger.LogInformation($"TodoReceiver Added: {item.Key}");
                    break;
                    case NotificationType.Remove:
                        logger.LogInformation($"TodoReceiver Removed: {item.Key}");
                    break;
                    case NotificationType.Clear:
                        logger.LogInformation("TodoReceiver Cleared all Todos");
                    break;
                }
                return Task.CompletedTask;
            });

        }

        /*
                    IList<StreamSubscriptionHandle<GeneratedEvent>> handles = await stream.GetAllSubscriptionHandles();
            if (handles.Count == 0)
            {
                await stream.SubscribeAsync(OnNextAsync);
            }
            else
            {
                foreach (StreamSubscriptionHandle<GeneratedEvent> handle in handles)
                {
                    await handle.ResumeAsync(OnNextAsync);
                }
            }

         */

    }
}