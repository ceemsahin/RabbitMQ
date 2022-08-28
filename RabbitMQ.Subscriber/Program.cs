// See https://aka.ms/new-console-template for more information
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared;
using System.Text;
using System.Text.Json;

var factory = new ConnectionFactory();

factory.Uri = new Uri("amqp://localhost:5672");


using var connectin = factory.CreateConnection();

var channel = connectin.CreateModel();



//publisher'da oluşturuldugu için tekrar yazmadık
//channel.QueueDeclare("hello-queue", true, false, false);


//var rabdomQueue = channel.QueueDeclare().QueueName;

//channel.QueueBind(rabdomQueue, "logs-fanout", "", null);
channel.ExchangeDeclare("header-exchange", durable: true, type: ExchangeType.Headers);

channel.BasicQos(0, 1, false);

var consumer = new EventingBasicConsumer(channel);

var queueName = channel.QueueDeclare().QueueName;

Dictionary<string, object> headers = new Dictionary<string, object>();

headers.Add("format", "pdf");
headers.Add("shape", "a4");
headers.Add("x-match", "any");

channel.QueueBind(queueName,"header-exchange",string.Empty,headers);
channel.BasicConsume(queueName, false, consumer);
Console.WriteLine("Loglar Dinleniyor");


consumer.Received += (object? sender, BasicDeliverEventArgs? e) =>
    {
        var message = Encoding.UTF8.GetString(e.Body.ToArray());

        Product product = JsonSerializer.Deserialize<Product>(message); 


        Console.WriteLine($"Gelen Mesaj: { product.Id}-{product.Name}-{product.Price}-{product.Stock}");
   
        channel.BasicAck(e.DeliveryTag, false);
    };



Console.ReadLine();

