// See https://aka.ms/new-console-template for more information
using RabbitMQ.Client;
using Shared;
using System.Text;
using System.Text.Json;

public enum LogNames
{

    Critical = 1,
    Error = 2,
    Warning = 3,
    Information = 4
}

class Program
{
    static void Main(string[] args)
    {
        var factory = new ConnectionFactory();

        factory.Uri = new Uri("amqp://localhost:5672");


        using var connectin = factory.CreateConnection();

        var channel = connectin.CreateModel();
        //channel.QueueDeclare("hello-queue", true, false, false);
        channel.ExchangeDeclare("header-exchange", durable: true, type: ExchangeType.Headers);

        Dictionary<string, object> headers = new Dictionary<string, object>();
        headers.Add("format", "pdf");
        headers.Add("shape", "a4");
        var properties = channel.CreateBasicProperties();

        properties.Headers = headers;
        properties.Persistent = true;

        var product = new Product { Id = 1, Name = "Mal Hatice", Price = 5, Stock = 1 };

        var productJsonString= JsonSerializer.Serialize(product);


        channel.BasicPublish("header-exchange", string.Empty, properties, Encoding.UTF8.GetBytes(productJsonString));

        Console.WriteLine( "Mesaj gönderilmiştir" );
        
        Console.ReadLine();

    }
}



