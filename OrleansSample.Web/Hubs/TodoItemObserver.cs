using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Orleans.Streams;
using OrleansSample.Interfaces.Models;

namespace OrleansSample.Web.Hubs
{
    public class TodoItemObserver : IAsyncObserver<TodoNotification>
    {
         private ILogger logger;
        private readonly Func<TodoNotification, Task> action;

        public TodoItemObserver(ILoggerFactory loggerFactory, Func<TodoNotification, Task> action)
        {
            this.logger = loggerFactory.CreateLogger($"{this.GetType().Name}");
            this.action = action;
        }
        public Task OnCompletedAsync() => Task.CompletedTask;

        public Task OnErrorAsync(Exception ex)
        {
            logger.LogError(ex, ex.Message);
            return Task.CompletedTask;
        }

        public Task OnNextAsync(TodoNotification item, StreamSequenceToken token = null) => action(item);
    }
}