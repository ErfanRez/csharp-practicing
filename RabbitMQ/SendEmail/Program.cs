using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Utils;

var queueName = "sendEmailForUser";
var exchangeName = "FinallyOrder";

var connectionFactory = new ConnectionFactory()
{
    HostName = "localHost",
    UserName = "guest",
    Password = "guest",
};

var connection = connectionFactory.CreateConnection();
var model = connection.CreateModel();
model.QueueDeclare(queueName, true, false, false, null);
model.ExchangeDeclare(exchangeName, ExchangeType.Fanout, true);


model.QueueBind(queueName, exchangeName, "", null);

var consumer = new EventingBasicConsumer(model);
consumer.Received += (sender, args) =>
{
    var result = Encoding.UTF8.GetString(args.Body.ToArray());
    var user = JsonConvert.DeserializeObject<Order>(result);
    if (user != null)
    {
        Console.WriteLine("Send Email For " + user.Email);
    }
};

model.BasicConsume(queueName, true, consumer);
Console.Read();