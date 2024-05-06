using Azure.Messaging.ServiceBus;
using EmailProvider.Service.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace EmailProvider.Functions
{
    public class EmailSender(ILogger<EmailSender> logger, IEmailService emailService)
    {
        private readonly ILogger<EmailSender> _logger = logger;
        private readonly IEmailService _emailService = emailService;

        [Function(nameof(EmailSender))]
        public async Task Run([ServiceBusTrigger("email_request", Connection = "ServiceBusConnection")] ServiceBusReceivedMessage message, ServiceBusMessageActions messageActions)
        {
            try
            {
                var emailRequest = _emailService.UnpackEmailRequest(message);

                if (emailRequest != null && !string.IsNullOrEmpty(emailRequest.To))
                {
                    if (_emailService.SendEmail(emailRequest))
                        await messageActions.CompleteMessageAsync(message);
                }

                await messageActions.DeadLetterMessageAsync(message);
            }
            catch (Exception ex)
            {
                _logger.LogError("ERROR EmailSender.run() : " + ex.Message);
            }
        }
    }
}
