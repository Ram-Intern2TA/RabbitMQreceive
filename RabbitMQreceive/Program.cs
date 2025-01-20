using System;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMQpractice
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                // Step 1: Create a connection to RabbitMQ
                var factory = new ConnectionFactory() { HostName = "localhost" };
                using (var connection = await factory.CreateConnectionAsync()) // Open connection
                using (var channel = await connection.CreateChannelAsync()) // Open channel
                {
                    // Step 2: Declare the exchange (same as producer)
                    /*await channel.ExchangeDeclareAsync(
                        exchange: "direct exchange",  // Exchange name
                        type: ExchangeType.Direct      // Exchange type: Direct
                    );*/

                    // Step 3: Declare the queue (same as producer)
                    await channel.QueueDeclareAsync(
                        queue: "sample2",              // Queue name
                        durable: true,                 // Durable queue
                        exclusive: false,              // Queue is not exclusive
                        autoDelete: false,             // Queue will not auto-delete
                        arguments: null                // No additional arguments
                    );

                    // Step 4: Bind the queue to the exchange (same as producer)
                   /* await channel.QueueBindAsync(
                        queue: "sample2",             // Queue name
                        exchange: "direct exchange",  // Exchange name
                        routingKey: "one"             // Routing key
                    );*/

                    // Step 5: Create a consumer to listen for messages
                    var consumer = new AsyncEventingBasicConsumer(channel);

                    // Step 6: Define logic for when a message is received
                    consumer.ReceivedAsync += async (model, ea) =>
                    {
                        var body = ea.Body.ToArray();  // Get the body of the message
                        var message = Encoding.UTF8.GetString(body);  // Convert byte array to string

                        // Step 7: Process the message (for example, print it)
                        Console.WriteLine($" [x] Received '{message}'");

                        // Simulate some async processing (e.g., saving to database or handling business logic)
                        await Task.Delay(1000);
                        Console.WriteLine($" [x] Processed '{message}'");
                    };

                    // Step 8: Start consuming messages from the "sample2" queue
                    await channel.BasicConsumeAsync(
                        queue: "sample2",  // Queue name
                        autoAck: false,     // Automatically acknowledge messages when received
                        consumer: consumer // The consumer that will process the messages
                    );

                    // Step 9: Keep the application running and wait for messages
                    Console.WriteLine(" [*] Waiting for messages. Press [Enter] to exit.");
                    Console.ReadLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            
        }
    }
}
