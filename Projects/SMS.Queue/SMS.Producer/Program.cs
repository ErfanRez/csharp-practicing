using MassTransit;
using SMS.Producer.Consumer;
using SMS.Producer.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var services = builder.Services;

services.AddMassTransit(x =>
{

    x.AddConsumer<BulkSmsConsumer>();


    x.UsingRabbitMq((context, cfg) =>
    {

        cfg.Host(new Uri("amqp://rabbit-queue:5672"), h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ReceiveEndpoint("web-service", e =>
        {
            e.ConfigureConsumer<BulkSmsConsumer>(context);
        });
    });
});

services.AddScoped<IMessageProcessor, SmsProcessor>();

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
