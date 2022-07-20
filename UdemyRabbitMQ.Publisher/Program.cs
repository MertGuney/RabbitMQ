using RabbitMQ.Client;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace UdemyRabbitMQ.Publisher
{
    public enum LogNames
    {
        Critical = 1,
        Error,
        Warning,
        Info
    }
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqps://okdmnwkm:LqWuSfuY66v1aZXnjHub5bBb-rWi4YKK@cattle.rmq2.cloudamqp.com/okdmnwkm");

            using var connection = factory.CreateConnection();

            var channel = connection.CreateModel();
            // 2. prop false olursa hafızada(memory) tutulur restart atarsa tüm kuyruk gider true olursa fiziksel kaydedilir restart atılsa bile gitmez
            // 3. prop true olursa sadece burada oluşturduğumuz kanal üzerinden bağlanabiliriz false olursa sub üzerinden farklı bir kanaldan bağlanabiliriz
            //channel.QueueDeclare("hello-queue", true, false, false);

            //channel.ExchangeDeclare("logs-fanout", durable: true, type: ExchangeType.Fanout);
            //channel.ExchangeDeclare("logs-direct", durable: true, type: ExchangeType.Direct);
            //channel.ExchangeDeclare("logs-topic", durable: true, type: ExchangeType.Topic);
            channel.ExchangeDeclare("header-exchange", durable: true, type: ExchangeType.Headers);

            //Enum.GetNames(typeof(LogNames)).ToList().ForEach(x =>
            //{



            //    var routeKey = $"route-{x}";
            //    var queueName = $"direct-queue-{x}";
            //    channel.QueueDeclare(queueName, true, false, false);
            //    channel.QueueBind(queueName, "logs-direct", routeKey, null);
            //});

            //Enumerable.Range(1, 50).ToList().ForEach(x =>
            //{
            //    //LogNames log = (LogNames)new Random().Next(1, 5);
            //    //string message = $"Message {x}";
            //    //string message = $"Log {x}";
            //    //string message = $"Log-Type: {log}";


            //    Random rnd = new Random();
            //    LogNames log1 = (LogNames)rnd.Next(1, 5);
            //    LogNames log2 = (LogNames)rnd.Next(1, 5);
            //    LogNames log3 = (LogNames)rnd.Next(1, 5);

            //    var routeKey = $"{log1}.{log2}.{log3}";
            //    string message = $"Log-Type: {log1}-{log2}-{log3}";
            //    var messageBody = Encoding.UTF8.GetBytes(message);
            //    //channel.BasicPublish(string.Empty, "hello-queue", null, messageBody);

            //    //channel.BasicPublish("logs-fanout", string.Empty, null, messageBody);

            //    //var routeKey = $"route-{log}";
            //    //channel.BasicPublish("logs-direct", routeKey, null, messageBody);

            //    channel.BasicPublish("logs-topic", routeKey, null, messageBody);


            //    //Console.WriteLine($"Mesaj Gönderilmiştir : {message}");
            //    Console.WriteLine($"Log Gönderilmiştir : {message}");
            //});

            Dictionary<string, object> headers = new Dictionary<string, object>();
            headers.Add("format", "pdf");
            headers.Add("shape", "a4");

            var properties = channel.CreateBasicProperties();
            properties.Headers = headers;
            properties.Persistent = true;//mesajlarda kalıcı hale gelir


            var product = new Product { Id = 1, Name = "Kalem", Price = 100, Stock = 10 };
            var productJsonString = JsonSerializer.Serialize(product);


            //channel.BasicPublish("header-exchange", string.Empty, properties, Encoding.UTF8.GetBytes("Header Mesajım"));
            channel.BasicPublish("header-exchange", string.Empty, properties, Encoding.UTF8.GetBytes(productJsonString));
            Console.WriteLine("Mesaj Gönderilmiştir.");

            Console.ReadLine();
        }
    }
}
