using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

class Program
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
                // Step 2: Declare the queues (same as producer)
                string[] queues = { "sample1", "sample2", "TestQueue", "hello" };

                foreach (var queue in queues)
                {
                    // Declare each queue (you can also add any specific queue properties you need)
                    await channel.QueueDeclareAsync(
                        queue: queue,
                        durable: true,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null
                    );
                }

                // Step 3: Create and start consumers for each queue
                var consumerTasks = new List<Task>();

                foreach (var queue in queues)
                {
                    var consumer = new AsyncEventingBasicConsumer(channel);

                    // Define logic for when a message is received
                    consumer.ReceivedAsync += async (model, ea) =>
                    {
                        var body = ea.Body.ToArray();  // Get the body of the message
                        var message = Encoding.UTF8.GetString(body);  // Convert byte array to string

                        // Step 4: Process the message
                        Console.WriteLine($" [x] Received from {queue}: '{message}'");

                        // Simulate async processing (e.g., saving to a database or handling business logic)
                        await Task.Delay(1000);
                        Console.WriteLine($" [x] Processed from {queue}: '{message}'");
                    };

                    // Start consuming messages from the queue
                    consumerTasks.Add(channel.BasicConsumeAsync(
                        queue: queue,
                        autoAck: false,  // Automatically acknowledge messages when received
                        consumer: consumer
                    ));
                }

                // Step 5: Wait for all consumers to finish processing
                await Task.WhenAll(consumerTasks);

                // Keep the application running and wait for messages
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
