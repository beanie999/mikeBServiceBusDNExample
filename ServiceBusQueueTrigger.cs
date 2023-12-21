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
    // Declare this as a non web transaction.
    [NewRelic.Api.Agent.Transaction(Web = false)]
    // Uses application settings for the queue name and connection.
    public void Run([ServiceBusTrigger("%SERVICE_BUS_QUEUE%", Connection = "SERVICE_BUS_CONNECTION")] ServiceBusReceivedMessage message)
    {
        // Get the current transaction from New Relic API
        NewRelic.Api.Agent.IAgent agent = NewRelic.Api.Agent.NewRelic.GetAgent();
        NewRelic.Api.Agent.ITransaction transaction = agent.CurrentTransaction;
        // Set the trace headers, which are stored within the application properties of the message.
        IEnumerable<string> Getter(ServiceBusReceivedMessage message, string key) 
        {
            try
            {
                return new string[] { (string) message.ApplicationProperties[key] };
            }
            catch
            {
                return null;
            }
        }
        transaction.AcceptDistributedTraceHeaders(message, Getter, NewRelic.Api.Agent.TransportType.Queue);

        // Add the queue name and time on the queue as custom attributes in the transaction.
        string queueName = System.Environment.GetEnvironmentVariable("SERVICE_BUS_QUEUE", EnvironmentVariableTarget.Process);
        TimeSpan timeOnQueue = DateTime.UtcNow - message.EnqueuedTime;
        transaction.AddCustomAttribute("queueName", queueName);
        transaction.AddCustomAttribute("timeOnQueueMilliSec", timeOnQueue.TotalMilliseconds);

        // Log the message details, which should appear within logs in context in New Relic.
        _logger.LogInformation($"Message Queue: {queueName}");
        _logger.LogInformation($"Message ID: {message.MessageId}");
        _logger.LogInformation($"Time on the queue in milli seconds: {timeOnQueue.TotalMilliseconds}");
        _logger.LogInformation($"Message Body: {message.Body}");
        foreach(KeyValuePair<string, object> kvp in message.ApplicationProperties)
        {
            _logger.LogInformation($"Key = {kvp.Key}, Value = {kvp.Value}");
        }
    }
}
