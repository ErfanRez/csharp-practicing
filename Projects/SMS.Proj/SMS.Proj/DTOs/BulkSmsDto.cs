namespace SMS.Proj.DTOs;

public class BulkSmsDto
{
    public required string Sender { get; set; }
    public required HashSet<string> Receiver { get; set; }
    public required string Text { get; set; }
}