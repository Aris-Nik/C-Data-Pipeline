using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using MySql.Data.MySqlClient;

ConnectionFactory factory = new();
factory.Uri = new Uri("amqp://guest:guest@localhost:5672");
factory.ClientProvidedName = "Rabbit Receiver1 App";

IConnection cnn = factory.CreateConnection();
IModel channel = cnn.CreateModel();

try{
    string exchangeName = "DemoExchange";
    string routingKey = "demo-routing-key";
    string queueName = "DemoQueue";

    channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
    channel.QueueDeclare(queueName, false, false, false, null);
    channel.QueueBind(queueName, exchangeName, routingKey);

    channel.BasicQos(0, 1, false);

    var consumer = new EventingBasicConsumer(channel);
    consumer.Received += (sender, args) => {

        Task.Delay(TimeSpan.FromSeconds(2)).Wait();
        var body = args.Body.ToArray();

        string message = Encoding.UTF8.GetString(body);

        Console.WriteLine($"Message Received: {message} and inserting it into DB");

        InsertMessagetoDB(message);
        

        channel.BasicAck(args.DeliveryTag, false);
    };

    string consumerTag = channel.BasicConsume(queueName, false, consumer);

    Console.ReadLine();

    channel.BasicCancel(consumerTag);
}
finally{
    cnn.Close();
    channel.Close();
}

void InsertMessagetoDB(string message)  {
     string connectionString = "Server=localhost;Database=sys;User ID=root;Password=my-secret-pw;";
     string[] record = message.Split(' ');
    //  int age = int.Parse(record[0]);
    //  string FirstName =;
    //  string LastName;
     string query = "INSERT INTO sys.Students(LastName, FirstName, Age) VALUES ('" + record[1] + "', '" + record[0] + "', " + record[2] +" );";
      try
        {
            // Create a connection to the database
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                // Open the connection
                connection.Open();
                Console.WriteLine("Connected to the database.");

                // Create a command to run the query
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    // Execute the command and read results
                    int rowsAffected = command.ExecuteNonQuery();
                    Console.WriteLine($"{rowsAffected} row(s) inserted.");
                }
            }
        }
        catch (MySqlException ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
        }
}


