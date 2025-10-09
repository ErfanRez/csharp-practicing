//

using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using Utils;

var exchangeName = "FinallyOrder";
var calculateExchangeName = "CalculateOrderFee";
var queueName = "HandleTeacherWallet";

Console.WriteLine("Enter Your PhoneNumber : ");
var userPhoneNumber = Console.ReadLine();

Console.WriteLine("Enter Your Email : ");
var userEmail = Console.ReadLine();

var connectionFactory = new ConnectionFactory()
{
    HostName = "localHost",
    UserName = "guest",
    Password = "guest",
};
var connection = connectionFactory.CreateConnection();
var model = connection.CreateModel();

model.ExchangeDeclare(exchangeName, ExchangeType.Fanout, true);
model.ExchangeDeclare(calculateExchangeName, ExchangeType.Direct, true);
model.QueueDeclare(queueName, true, false, false);

model.QueueBind(queueName,calculateExchangeName,queueName);

var user = new Order()
{
    Email = userEmail ?? "",
    PhoneNumber = userPhoneNumber,
    CourseId = new List<long>()
    {
        1,2, 3, 4,
    }
};
var userConverted = JsonConvert.SerializeObject(user);
var body = Encoding.UTF8.GetBytes(userConverted);
var isSend = true;
do
{
    model.BasicPublish(exchangeName, "", null, body);
    model.BasicPublish(calculateExchangeName, queueName, null, body);

    Console.WriteLine("Send Again ? ");
    var res = Console.ReadLine();
    if (res != "y")
    {
        isSend = false;
    }
} while (isSend);

