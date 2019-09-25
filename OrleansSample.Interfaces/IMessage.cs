using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrleansSample.Interfaces
{
    public interface IMessage : Orleans.IGrainWithIntegerKey
    {
         Task<string> SendMessage(string msg);

        Task<IEnumerable<string>> GetMessages();

        Task RemoveMessage(int position);

        Task Subscribe(IObserver observer);
        Task UnSubscribe(IObserver observer);
    }
}