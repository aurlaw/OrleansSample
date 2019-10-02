using System.Threading.Tasks;
using Orleans;

namespace OrleansSample.Interfaces
{
    public interface IObserverManger : IGrainWithIntegerKey
    {
        Task Subscribe(IObserver observer);
        Task UnSubscribe(IObserver observer);

        Task NotifyObserver(string message); 
         
    }
}