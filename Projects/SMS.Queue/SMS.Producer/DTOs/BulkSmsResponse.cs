namespace SMS.Producer.DTOs;

public class BulkSmsResponse
{
    public required long SequenceNumber { get; set; }
    public required bool[] SentStatus { get; set; }
}
