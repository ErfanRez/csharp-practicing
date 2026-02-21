using SMS.Producer.DTOs;
using System.Collections.Concurrent;

namespace SMS.Producer.Data;

internal sealed class QueueData
{

    public static readonly ConcurrentDictionary<long, PendingRequest> PendingRequests = new();
}

public class PendingRequest
{
    public TaskCompletionSource<BulkSmsResponse> CompletionSource { get; } = new();
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
    public BulkSmsResponse Response { get; set; }
}