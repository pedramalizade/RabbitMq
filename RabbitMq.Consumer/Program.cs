using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

await channel.QueueDeclareAsync(
    queue: "message",
    durable: true,
    exclusive: false,
    autoDelete: false,
    arguments: null);

Console.WriteLine("Waiting for messages...");

var consumer = new AsyncEventingBasicConsumer(channel);
consumer.ReceivedAsync += async (sender, eventArgs) =>
{
    byte[] body = eventArgs.Body.ToArray(); 
    string message = Encoding.UTF8.GetString(body);

    Console.WriteLine($"Recived: {message}");

   await ((AsyncEventingBasicConsumer)sender).Channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false);
};

await channel.BasicConsumeAsync("message", autoAck: false, consumer);
Console.ReadLine();