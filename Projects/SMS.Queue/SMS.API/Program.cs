using RabbitMQ.Client;
using SMS.API.Workers;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//builder.Services.AddMassTransit(x =>
//{
//    //x.AddConsumer<SingleSmsConsumer>();

//    x.UsingRabbitMq((context, cfg) =>
//    {

//        cfg.Host(new Uri("amqp://sms-queue:5672"), h =>
//        {
//            h.Username("guest");
//            h.Password("guest");
//        });

//        //cfg.ReceiveEndpoint("sms-messages-queue", e =>
//        //{
//        //    //e.ConfigureConsumer<SingleSmsConsumer>(context);
//        //});
//    });
//});

builder.Services.AddSingleton(sp =>
{
    var factory = new ConnectionFactory
    {
        HostName = "rabbit.queue",
        UserName = "guest",
        Password = "guest"
    };


    return factory.CreateConnectionAsync().GetAwaiter().GetResult();
});


builder.Services.AddSingleton<IChannel>(sp =>
{
    var conn = sp.GetRequiredService<IConnection>();
    var channel = conn.CreateChannelAsync().GetAwaiter().GetResult();

    channel.QueueDeclareAsync(queue: "sms_queue",
                              durable: false,
                              exclusive: false,
                              autoDelete: false,
                              arguments: null).GetAwaiter().GetResult();

    return channel;
});


builder.Services.AddSingleton<ProducerService>();
builder.Services.AddSingleton<ConsumerManagerService>();

builder.Services.AddHostedService(sp => sp.GetRequiredService<ProducerService>());
builder.Services.AddHostedService(sp => sp.GetRequiredService<ConsumerManagerService>());


var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
