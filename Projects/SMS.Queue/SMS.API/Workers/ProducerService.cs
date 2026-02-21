using RabbitMQ.Client;
using System.Text;

namespace SMS.API.Workers;

public class ProducerService : BackgroundService
{
    private readonly IChannel _channel;
    private readonly ILogger<ProducerService> _logger;
    private int _messagesPerCall = 0;

    public ProducerService(IChannel channel, ILogger<ProducerService> logger)
    {
        _channel = channel;
        _logger = logger;
    }

    public void SetMessageCount(int count)
    {
        _messagesPerCall = count;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Producer started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            if (_messagesPerCall > 0)
            {
                _logger.LogInformation($"Producing {_messagesPerCall} messages.");

                for (int i = 1; i <= _messagesPerCall; i++)
                {
                    var body = Encoding.UTF8.GetBytes($"Message {i} at {DateTime.Now}");

                    await _channel.BasicPublishAsync(
                        exchange: "",
                        routingKey: "sms_queue",
                        body: body);
                }

                _logger.LogInformation("Produced messages.");
                _messagesPerCall = 0;
            }

            await Task.Delay(1000, stoppingToken);
        }
    }
}


