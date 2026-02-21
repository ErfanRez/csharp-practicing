using Microsoft.AspNetCore.Mvc;
using SMS.API.Workers;

namespace SMS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WorkerController : ControllerBase
{
    private readonly ProducerService _producerService;
    private readonly ConsumerManagerService _consumerManager;

    public WorkerController(ProducerService producerService, ConsumerManagerService consumerManager)
    {
        _producerService = producerService;
        _consumerManager = consumerManager;
    }

    [HttpPost("send/{count}")]
    public IActionResult Send(int count)
    {
        _producerService.SetMessageCount(count);
        return Ok($"Will produce {count} messages.");
    }

    [HttpPost("start/{count}")]
    public IActionResult StartConsumers(int count)
    {
        _consumerManager.SetConsumerCount(count);
        return Ok($"Will run {count} consumers.");
    }
}
