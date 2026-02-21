using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SMS.Consumer.Services;

namespace SMS.Consumer;

class Program
{
    static async Task Main(string[] args)
    {
        var host = new HostBuilder()
            .ConfigureServices(services =>
            {
                services.AddHostedService<MyBackgroundService>();
                services.AddMassTransit(x =>
                {
                    x.AddConsumer<SingleSmsConsumer>();

                    x.UsingRabbitMq((context, cfg) =>
                    {

                        cfg.Host(new Uri("amqp://rabbit-queue:5672"), h =>
                        {
                            h.Username("guest");
                            h.Password("guest");
                        });

                        cfg.ReceiveEndpoint("part-collector", e =>
                        {
                            e.ConfigureConsumer<SingleSmsConsumer>(context);
                        });
                    });
                });
            })
            .Build();

        await host.RunAsync();
    }
}

public class MyBackgroundService : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            Console.WriteLine($"Background service working at {DateTime.Now}");
            await Task.Delay(5000, ct);
        }
    }
}