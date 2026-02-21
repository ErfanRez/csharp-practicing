namespace SMS.Common;

public sealed class BulkSmsIsProcessed
{
    public required long SequenceNumber { get; init; }
    public required bool[] SentStatus { get; init; }
}
