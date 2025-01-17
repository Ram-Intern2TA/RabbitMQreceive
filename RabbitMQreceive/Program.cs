using System;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

class Program
{
    static async Task Main(string[] args)
    {
        // Step 1: Create a connection to RabbitMQ
        var factory = new ConnectionFactory { HostName = "localhost" };
        var connection = await factory.CreateConnectionAsync(); // Asynchronously create a connection
        var channel = await connection.CreateChannelAsync(); // Asynchronously create a channel

        // Step 2: Declare a queue named "hello"
        await channel.QueueDeclareAsync(
            queue: "hello",          // Queue name
            durable: false,          // Queue is not durable (not persistent across restarts)
            exclusive: false,        // Queue is not exclusive to this connection
            autoDelete: false,       // Queue will not be automatically deleted
            arguments: null          // No additional arguments
        );

        Console.WriteLine(" [*] Waiting for messages.");

        // Step 3: Create a consumer to listen for messages
        var consumer = new AsyncEventingBasicConsumer(channel);

        // Step 4: Define the logic to process messages when received
        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();  // Get the body of the message
            var message = Encoding.UTF8.GetString(body);  // Convert the byte array to a string
            Console.WriteLine($" [x] Received {message}");  // Print the received message
            await Task.CompletedTask;  // Mark the message handling as complete
        };

        // Step 5: Start consuming messages from the "hello" queue
        await channel.BasicConsumeAsync("hello", autoAck: true, consumer: consumer);

        // Step 6: Wait for the user to press Enter before exiting
        Console.WriteLine(" Press [enter] to exit.");
        Console.ReadLine();
    }
}
