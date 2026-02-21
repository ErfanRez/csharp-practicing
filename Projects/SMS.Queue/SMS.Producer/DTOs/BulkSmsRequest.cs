namespace SMS.Producer.DTOs;

public class BulkSmsRequest
{
    public required string Sender { get; set; }
    public required HashSet<string> Receivers { get; set; }
    public int? Delay { get; set; }
    public required string Message { get; set; }
    public required long SequenceNumber { get; set; }

}
