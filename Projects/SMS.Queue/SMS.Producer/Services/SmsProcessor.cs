using MassTransit;
using SMS.Common;
using SMS.Producer.Data;
using SMS.Producer.DTOs;

namespace SMS.Producer.Services;

public interface IMessageProcessor
{
    Task<BulkSmsResponse> ProcessMessageAsync(BulkSmsRequest message, CancellationToken ct);
}

public class SmsProcessor : IMessageProcessor
{
    private readonly IBus _bus;
    private readonly ILogger<SmsProcessor> _logger;

    public SmsProcessor(IBus bus, ILogger<SmsProcessor> logger)
    {
        _bus = bus;
        _logger = logger;
    }

    public async Task<BulkSmsResponse> ProcessMessageAsync(BulkSmsRequest message, CancellationToken ct)
    {
        var sequenceNumber = message.SequenceNumber;
        var results = new List<bool>();

        var pendingRequest = new PendingRequest();

        if (!QueueData.PendingRequests.TryAdd(sequenceNumber, pendingRequest))
        {
            _logger.LogWarning("Sequence number {SequenceNumber} already exists", sequenceNumber);
            throw new InvalidOperationException($"Sequence number {sequenceNumber} is already being processed");
        }

        try
        {
            _logger.LogInformation("Starting to process bulk SMS for sequence {SequenceNumber}", sequenceNumber);

            var partCollectorEndpoint = await _bus.GetSendEndpoint(new Uri("queue:part-collector"));

            foreach (var receiver in message.Receivers)
            {
                await partCollectorEndpoint.Send(new SendSingleSms
                {
                    Sender = message.Sender,
                    Receiver = receiver,
                    Message = message.Message,
                    SequenceNumber = message.SequenceNumber,
                    TotalCount = message.Receivers.Count
                });

                if (message.Delay.HasValue && message.Delay.Value > 0)
                {
                    await Task.Delay(TimeSpan.FromSeconds(message.Delay.Value), ct);
                }
            }

            _logger.LogInformation("All messages sent for sequence {SequenceNumber}, waiting for processing to complete", sequenceNumber);

            var timeout = TimeSpan.FromSeconds(message.Delay.HasValue ? message.Delay.Value + 10 : 10);
            var completedTask = await Task.WhenAny(
                pendingRequest.CompletionSource.Task,
                Task.Delay(timeout, ct)
            );

            if (completedTask == pendingRequest.CompletionSource.Task)
            {
                var response = await pendingRequest.CompletionSource.Task;
                _logger.LogInformation("Successfully received processed notification for sequence {SequenceNumber}", sequenceNumber);
                return response;
            }
            else
            {
                _logger.LogWarning("Timeout waiting for processing to complete for sequence {SequenceNumber}", sequenceNumber);

                QueueData.PendingRequests.TryRemove(sequenceNumber, out _);


                throw new TimeoutException($"Processing timed out for sequence number {sequenceNumber}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing bulk SMS for sequence {SequenceNumber}", sequenceNumber);
            QueueData.PendingRequests.TryRemove(sequenceNumber, out _);

            throw;
        }
    }
}