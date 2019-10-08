using System.Threading.Tasks;
using Orleans;

namespace OrleansSample.Interfaces
{
    public interface IBlobStorageGrain  : IGrainWithGuidKey, IGrainInterfaceMarker
    {
         Task<string> UploadAsync(string name, string container, byte[] data);
         
    }
}