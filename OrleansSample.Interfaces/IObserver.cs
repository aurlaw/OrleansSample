using Orleans;

namespace OrleansSample.Interfaces
{
    public interface IObserver : IGrainObserver
    {
        void ReceiveMessage(string message);

    }
}
