using Orleans;
using Orleans.Providers;
using OrleansSample.Interfaces;
using Orleans.Streams;
using System.Collections.Generic;
using System.Threading.Tasks;
using OrleansSample.Interfaces.Models;
using Microsoft.Extensions.Logging;
using OrleansSample.Interfaces.Service;

namespace OrleansSample.Grains
{
    [StorageProvider(ProviderName = Constants.StorageName)]
    public class TodoGrain : Grain<TodoState>, ITodo, IGrainMarker
    {
        private ILogger logger;
        private IBlobStorage storage;
        
        public TodoGrain(ILoggerFactory loggerFactory,IBlobStorage storage)
        {
            this.logger = loggerFactory.CreateLogger($"{this.GetType().Name}-{this.IdentityString}");
            this.storage = storage;
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

        public async Task SetAsync(TodoItem item, TodoImageUpload imageUpload = null)
        {
            logger.LogInformation($"adding todo item: {item.Key}");
            if(imageUpload != null) 
            {
                item.ImageUrl = await storage.Upload(imageUpload.ImageName, Constants.StorageContainer, imageUpload.ImageData);
            }
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
