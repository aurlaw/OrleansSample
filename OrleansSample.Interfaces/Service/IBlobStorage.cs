using System.Threading.Tasks;

namespace OrleansSample.Interfaces.Service
{
    public interface IBlobStorage
    {
         Task<string> Upload(string name, string container, byte[] data);
    }
}