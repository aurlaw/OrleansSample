using System.Threading.Tasks;
using OrleansSample.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Text;

namespace OrleansSample.Grains
{

    public class FakeEmailSender : IEmailSender
    {
        private readonly ILogger<FakeEmailSender> _logger;

        public FakeEmailSender(ILogger<FakeEmailSender> logger)
        {
            _logger = logger;
        }

        public Task SendEmailAsync(string from, string[] to, string subject, string body)
        {
            var emailBuilder = new StringBuilder();
            emailBuilder.AppendLine("Sending new Email...");
            emailBuilder.AppendLine();
            emailBuilder.AppendLine($"From: {from}");
            emailBuilder.AppendLine($"To: {string.Join(", ", to)}");
            emailBuilder.AppendLine($"Subject: {subject}");
            emailBuilder.AppendLine($"Body: {Environment.NewLine}{body}");

            _logger.LogInformation(emailBuilder.ToString());

            return Task.CompletedTask;        }
    }
}