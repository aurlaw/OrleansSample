using System.Threading.Tasks;
using Orleans;

namespace OrleansSample.Interfaces
{
    public interface IEverythingIsOkGrain : IGrainWithStringKey, IRemindable
    {
         Task Start();
         Task Stop();
    }
}