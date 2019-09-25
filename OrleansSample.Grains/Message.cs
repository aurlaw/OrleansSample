using Orleans;
using Orleans.Providers;
using OrleansSample.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace OrleansSample.Grains
{
    [StorageProvider(ProviderName = "OrleansStorage")]
    public class Message : Grain<MessageArchive>, IMessage
    {
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
            }
        }


        public async Task<string> SendMessage(string msg)
        {
             State.Messages.Add(msg);

             await WriteStateAsync();

             return $"Message: '{msg}'";
        }
    }

    public class MessageArchive
    {
        public List<string> Messages { get; } = new List<string>();
    }

}