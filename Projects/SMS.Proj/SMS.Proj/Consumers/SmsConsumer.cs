using MassTransit;
using SMS.Gateway.DTOs;

namespace SMS.Proj.Consumers
{
    public class SingleSmsConsumer : IConsumer<SingleSMSDto>
    {
        private readonly ILogger<SingleSmsConsumer> _logger;

        public SingleSmsConsumer(ILogger<SingleSmsConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<SingleSMSDto> context)
        {
            var message = context.Message;
            _logger.LogInformation("Received SMS: From {Sender} to {Receiver}",
                message.Sender, message.Receiver);
            return Task.CompletedTask;
        }
    }
}
