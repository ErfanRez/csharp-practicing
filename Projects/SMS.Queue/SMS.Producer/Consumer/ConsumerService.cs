using MassTransit;
using SMS.Common;
using SMS.Producer.Data;
using SMS.Producer.DTOs;

namespace SMS.Producer.Consumer;

public class BulkSmsConsumer : IConsumer<BulkSmsIsProcessed>
{
    private readonly ILogger<BulkSmsConsumer> _logger;

    public BulkSmsConsumer(ILogger<BulkSmsConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<BulkSmsIsProcessed> context)
    {
        var sequenceNumber = context.Message.SequenceNumber;

        _logger.LogInformation("Received processed notification for sequence {SequenceNumber}", sequenceNumber);

        var response = new BulkSmsResponse
        {
            SequenceNumber = sequenceNumber,
            SentStatus = context.Message.SentStatus
        };

        if (QueueData.PendingRequests.TryRemove(sequenceNumber, out var pendingRequest))
        {
            _logger.LogInformation("Completing pending request for sequence {SequenceNumber}", sequenceNumber);

            pendingRequest.Response = response;
            pendingRequest.CompletionSource.TrySetResult(response);
        }
        else
        {
            _logger.LogWarning("No pending request found for sequence {SequenceNumber}", sequenceNumber);
        }

        return Task.CompletedTask;
    }
}