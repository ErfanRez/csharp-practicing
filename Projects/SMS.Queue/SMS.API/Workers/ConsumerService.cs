using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace SMS.API.Workers;

public class ConsumerManagerService : BackgroundService
{
    private readonly IConnection _connection;
    private readonly ILogger<ConsumerManagerService> _logger;
    private readonly List<Task> _consumerTasks = new();
    private CancellationTokenSource _cts = new();

    public ConsumerManagerService(IConnection connection, ILogger<ConsumerManagerService> logger)
    {
        _connection = connection;
        _logger = logger;
    }

    public void SetConsumerCount(int count)
    {
        _cts.Cancel();
        _consumerTasks.Clear();
        _cts = new CancellationTokenSource();

        for (int i = 0; i < count; i++)
        {
            _consumerTasks.Add(Task.Run(async () =>
            {
                var channel = await _connection.CreateChannelAsync();
                await channel.QueueDeclareAsync(queue: "sms_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);

                var consumer = new AsyncEventingBasicConsumer(channel);
                consumer.ReceivedAsync += async (model, ea) =>
                {
                    var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                    _logger.LogInformation($"Consumer got: {message}");
                    await channel.BasicAckAsync(ea.DeliveryTag, false);
                    await Task.Yield();
                };

                await channel.BasicConsumeAsync(queue: "sms_queue", autoAck: false, consumer: consumer);

                await Task.Delay(-1, _cts.Token);
            }));
        }
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Consumer Manager started.");
        return Task.CompletedTask;
    }
}

