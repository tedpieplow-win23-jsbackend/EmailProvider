using Azure.Communication.Email;
using EmailProvider.Service;
using EmailProvider.Service.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddSingleton<IEmailService, EmailService>();
        services.AddSingleton(new EmailClient(Environment.GetEnvironmentVariable("CommunicationServices")));
    })
    .Build();

host.Run();
