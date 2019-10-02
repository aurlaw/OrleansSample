using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans;

namespace OrleansSample.Interfaces
{
    public interface IMessage : IGrainWithIntegerKey, IObserverManger
    {
        Task<string> SendMessage(string msg);
        Task<IEnumerable<string>> GetMessages();
        Task<string> GetMessage(int position);
        Task RemoveMessage(int position);
    }
}