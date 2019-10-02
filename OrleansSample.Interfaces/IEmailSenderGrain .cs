using System.Threading.Tasks;
using Orleans;

namespace OrleansSample.Interfaces
{
    public interface IEmailSenderGrain : IGrainWithGuidKey, IGrainInterfaceMarker
    {
         Task SendEmail(string from, string[] to, string subject, string body);
    }
}