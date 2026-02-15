using MassTransit;
using SMS.Proj.Consumers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit(x =>
{
    //x.AddConsumer<SingleSmsConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {

        cfg.Host(new Uri("amqp://sms-queue:5672"), h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        //cfg.ReceiveEndpoint("sms-messages-queue", e =>
        //{
        //    //e.ConfigureConsumer<SingleSmsConsumer>(context);
        //});
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
