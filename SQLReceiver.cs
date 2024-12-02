using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using MySql.Data.MySqlClient;

string connectionString = "Server=localhost;Database=sys;User ID=root;Password=my-secret-pw;";
string query = "select * from Students";
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
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    // Assuming you have columns 'id' and 'name' in 'mytable'
                    Console.WriteLine($"ID: {reader["StudentID"]}, Name: {reader["FirstName"]} {reader["LastName"]}");
                }
            }
        }
    }
}
catch (MySqlException ex)
{
    Console.WriteLine("An error occurred: " + ex.Message);
}
