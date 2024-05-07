using Azure.Messaging.ServiceBus;
using EmailProvider.Models;

namespace EmailProvider.Service.Interfaces
{
    public interface IEmailService
    {
        bool SendEmail(EmailRequest emailRequest);
        EmailRequest UnpackEmailRequest(ServiceBusReceivedMessage message);
    }
}