using System.Threading.Tasks;

namespace OrleansSample.Interfaces
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string from, string[] to, string subject, string body);

    }
}