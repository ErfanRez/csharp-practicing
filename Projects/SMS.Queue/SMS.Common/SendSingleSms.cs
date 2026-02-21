namespace SMS.Common;

public sealed class SendSingleSms
{
    public required string Sender { get; init; }
    public required string Receiver { get; init; }
    public required string Message { get; init; }
    public required long SequenceNumber { get; init; }
    public required int TotalCount { get; set; }
}
