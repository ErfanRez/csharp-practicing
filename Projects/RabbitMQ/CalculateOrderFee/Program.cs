using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Utils;

var queueName = "HandleTeacherWallet";

var connectionFactory = new ConnectionFactory()
{
    HostName = "localHost",
    UserName = "guest",
    Password = "guest",
};
var connection = connectionFactory.CreateConnection();
var model = connection.CreateModel();

model.QueueDeclare(queueName, true, false, false, null);


var consumer = new EventingBasicConsumer(model);
consumer.Received += (sender, args) =>
{
    var result = Encoding.UTF8.GetString(args.Body.ToArray());
    var order = JsonConvert.DeserializeObject<Order>(result);

    if (true)
    {
        Console.WriteLine($"Teacher Wallet Charged => {order.CourseId[0]}");
        model.BasicAck(args.DeliveryTag, false);
    }
};
model.BasicConsume(queueName, false, consumer);
Console.Read();