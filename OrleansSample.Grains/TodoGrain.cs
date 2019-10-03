using Orleans;
using Orleans.Providers;
using OrleansSample.Interfaces;
using Orleans.Streams;
using System.Collections.Generic;
using System.Threading.Tasks;
using OrleansSample.Interfaces.Models;
using Microsoft.Extensions.Logging;

namespace OrleansSample.Grains
{
    [StorageProvider(ProviderName = Constants.StorageName)]
    public class TodoGrain : Grain<TodoState>, ITodo, IGrainMarker
    {
        private ILogger logger;
        
        public TodoGrain(ILoggerFactory loggerFactory)
        {
            this.logger = loggerFactory.CreateLogger($"{this.GetType().Name}-{this.IdentityString}");
        }
        public async Task ClearAsync()
        {
            logger.LogInformation("clear todo items");
            State.Items.Clear();
            await ClearStateAsync();
            // notify listers
            GetStreamProvider(Constants.StreamProvider)
                .GetStream<TodoNotification>(this.GetPrimaryKey(), nameof(ITodo))
                .OnNextAsync(new TodoNotification(NotificationType.Clear))
                .Ignore();

        }

        public Task<IEnumerable<TodoItem>> GetAllAsync()
        {
            return Task.FromResult<IEnumerable<TodoItem>>(State.Items);
        }

        public async Task SetAsync(TodoItem item)
        {
            logger.LogInformation($"adding todo item: {item.Key}");
            State.Items.Add(item);
            await WriteStateAsync();

            // notify listers
            GetStreamProvider(Constants.StreamProvider)
                .GetStream<TodoNotification>(this.GetPrimaryKey(), nameof(ITodo))
                .OnNextAsync(new TodoNotification(item.Key, item, NotificationType.Add))
                .Ignore();
        }
    }

    public class TodoState 
    {
        public List<TodoItem> Items {get;} = new List<TodoItem>();
    }
}
