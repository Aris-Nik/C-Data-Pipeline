using RabbitMQ.Client;
using System.Text;

ConnectionFactory factory = new();
factory.Uri = new Uri("amqp://guest:guest@localhost:5672");
factory.ClientProvidedName = "Rabbit Sender App";

IConnection cnn = factory.CreateConnection();
IModel channel = cnn.CreateModel();
Random random= new Random();

try{
    string exchangeName = "DemoExchange";
    string routingKey = "demo-routing-key";
    string queueName = "DemoQueue";

    channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
    channel.QueueDeclare(queueName, false, false, false, null);
    channel.QueueBind(queueName, exchangeName, routingKey);


    for (int i = 0; i < 4; i++){
        string message = "Name"+ i + " LastName" + i + " " + random.Next(15, 50);
        Console.WriteLine(message);
        byte[] messageBodyBytes = Encoding.UTF8.GetBytes(message);
        channel.BasicPublish(exchangeName, routingKey, null, messageBodyBytes);
        Thread.Sleep(1000);
    }
}
finally{
    channel.Close();
    cnn.Close();
}