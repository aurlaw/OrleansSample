using System.Threading.Tasks;
using Orleans;
using OrleansSample.Interfaces;

namespace OrleansSample.Grains
{
    public class EmailSenderGrain : Grain, IEmailSenderGrain, IGrainMarker
    {
        private readonly IEmailSender _emailSender;

        public EmailSenderGrain(IEmailSender sender) 
        {
            _emailSender = sender;
        }
        public async Task SendEmail(string from, string[] to, string subject, string body)
        {
            await _emailSender.SendEmailAsync(from, to, subject, body);
        }
    }
}