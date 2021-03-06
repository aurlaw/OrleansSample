using Orleans;
using Orleans.Providers;
using OrleansSample.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans.Concurrency;

namespace OrleansSample.Grains
{
    [StorageProvider(ProviderName = Constants.StorageName)]
    public class MessageGrain : Grain<MessageArchive>, IMessage, IGrainMarker
    {
        private  ObserverSubscriptionManager<IObserver> _subsManager;

        public override async Task OnActivateAsync() 
        {
            // We created the utility at activation time.
            _subsManager = new ObserverSubscriptionManager<IObserver>();
            await base.OnActivateAsync();
        }

        public Task<IEnumerable<string>> GetMessages()
        {
            return Task.FromResult<IEnumerable<string>>(State.Messages);
        }

        public async Task RemoveMessage(int position) 
        {
            if(position >= 0 && position < State.Messages.Count) 
            {
                State.Messages.RemoveAt(position);
                await WriteStateAsync();

                await NotifyObserver($"{position} removed");

            }
        }

        public async Task<string> SendMessage(string msg)
        {
             State.Messages.Add(msg);

             await WriteStateAsync();

                await NotifyObserver($"{msg} added");

             return $"Message: '{msg}'";
        }

        public Task<string> GetMessage(int position) 
        {
            if(position >= 0 && position < State.Messages.Count) 
            {
                return Task.FromResult(State.Messages[position]);

            }
            return Task.FromResult(string.Empty);
        }

        public Task Subscribe(IObserver observer)
        {
            if(!_subsManager.IsSubscribed(observer))
            {
                _subsManager.Subscribe(observer);
            }
            return Task.CompletedTask;
        }

        public Task UnSubscribe(IObserver observer)
        {
            if(_subsManager.IsSubscribed(observer))
            {
                _subsManager.Unsubscribe(observer);
            }
            return Task.CompletedTask;
        }
        public Task NotifyObserver(string message) 
        {
            _subsManager.Notify(s => s.ReceiveMessage(message));
            return Task.CompletedTask;
        }

    }

    public class MessageArchive
    {
        public List<string> Messages { get; } = new List<string>();
    }

}