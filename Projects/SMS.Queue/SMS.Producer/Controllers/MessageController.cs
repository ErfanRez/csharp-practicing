using Microsoft.AspNetCore.Mvc;
using SMS.Producer.DTOs;
using SMS.Producer.Services;

namespace SMS.Producer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MessageController : ControllerBase
{
    private readonly ILogger<MessageController> _logger;
    private readonly IMessageProcessor _messageProcessor;
    public MessageController(ILogger<MessageController> logger, IMessageProcessor messageProcessor)
    {
        _logger = logger;
        _messageProcessor = messageProcessor;
    }


    [HttpPost]
    public async Task<IActionResult> Post([FromBody] BulkSmsRequest request, CancellationToken ct)
    {
        var result = await _messageProcessor.ProcessMessageAsync(request, ct);


        return Ok(result);
    }
}
