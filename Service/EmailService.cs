﻿using Azure;
using Azure.Communication.Email;
using Azure.Messaging.ServiceBus;
using EmailProvider.Functions;
using EmailProvider.Models;
using EmailProvider.Service.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace EmailProvider.Service;

public class EmailService(EmailClient emailClient, ILogger<EmailSender> logger) : IEmailService
{
    private readonly EmailClient _emailClient = emailClient;
    private readonly ILogger<EmailSender> _logger = logger;

    public EmailRequest UnpackEmailRequest(ServiceBusReceivedMessage message)
    {
        try
        {
            var emailRequest = JsonSerializer.Deserialize<EmailRequest>(message.Body.ToString());
            if (emailRequest != null)
            {
                return emailRequest;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("ERROR EmailSender.UnpackEmailRequest() : " + ex.Message);
        }

        return null!;
    }

    public bool SendEmail(EmailRequest emailRequest)
    {
        try
        {
            var result = _emailClient.Send(WaitUntil.Completed,
                    senderAddress: Environment.GetEnvironmentVariable("SenderAddress"),
                    recipientAddress: emailRequest.To,
                    subject: emailRequest.Subject,
                    htmlContent: emailRequest.HtmlBody,
                    plainTextContent: emailRequest.PlainText
                );

            if (result.HasCompleted)
                return true;

        }
        catch (Exception ex)
        {

            _logger.LogError("ERROR EmailSender.SendEmail() : " + ex.Message);
        }

        return false;
    }
}