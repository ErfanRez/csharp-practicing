namespace SMS.Gateway.DTOs;

public class SingleSMSDto
{
    public required string Sender { get; set; }
    public required string Receiver { get; set; }
    public required string Text { get; set; }
}
