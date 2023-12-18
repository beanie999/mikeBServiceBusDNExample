using System;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace NewRelic.Function;

public class ServiceBusQueueTrigger
{
    private readonly ILogger<ServiceBusQueueTrigger> _logger;

    public ServiceBusQueueTrigger(ILogger<ServiceBusQueueTrigger> logger)
    {
        _logger = logger;
    }

    [Function(nameof(ServiceBusQueueTrigger))]
    public void Run([ServiceBusTrigger("mbean-test-queue", Connection = "mbeantestservicebus_SERVICEBUS")] ServiceBusReceivedMessage message)
    {
        _logger.LogInformation("Message ID: {id}", message.MessageId);
        _logger.LogInformation("Message Body: {body}", message.Body);
        _logger.LogInformation("Message Content-Type: {contentType}", message.ContentType);
    }
}
