using MassTransit;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using SMS.API.DTOs;
using System.Text;

[ApiController]
[Route("api/sms")]
public class SmsController : ControllerBase
{
    private readonly IChannel _channel;
    private readonly IPublishEndpoint _publishEndpoint;

    public SmsController(IChannel channel, IPublishEndpoint publishEndpoint)
    {
        _channel = channel;
        _publishEndpoint = publishEndpoint;
    }

    [HttpPost]
    public IActionResult SendSingleSms([FromBody] SingleSMSDto request)
    {
        if (request == null || string.IsNullOrEmpty(request.Sender) || string.IsNullOrEmpty(request.Receiver))
        {
            return BadRequest("Invalid request.");
        }

        _publishEndpoint.Publish(request);

        return Ok("Single SMS sent successfully.");
    }

    [HttpPost("/bulk")]
    public IActionResult SendSms([FromBody] BulkSmsDto request)
    {
        if (request == null || string.IsNullOrEmpty(request.Sender) || request.Receiver.Count == 0)
        {
            return BadRequest("Invalid request.");
        }

        foreach (var item in request.Receiver)
        {
            var singleRequest = new SingleSMSDto
            {
                Sender = request.Sender,
                Receiver = item,
                Text = request.Text
            };
            _publishEndpoint.Publish(singleRequest);
        }

        return Ok("Bulk SMS sent successfully.");
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendBulkSms([FromBody] int count)
    {
        for (int i = 0; i < count; i++)
        {
            var dto = new SingleSMSDto
            {
                Sender = $"Sender{i + 1}",
                Receiver = $"Receiver{i + 1}",
                Text = $"This is message number {i + 1}"
            };

            var jsonMessage = System.Text.Json.JsonSerializer.Serialize(dto);

            var body = Encoding.UTF8.GetBytes(jsonMessage);

            await _channel.BasicPublishAsync(
                exchange: "",
                routingKey: "sms_queue",
                body: body);
        }

        return Ok(new { Message = "Messages published" });
    }
}
