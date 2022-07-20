using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;

namespace UdemyRabbitMQ.Subscriber
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqps://okdmnwkm:LqWuSfuY66v1aZXnjHub5bBb-rWi4YKK@cattle.rmq2.cloudamqp.com/okdmnwkm");

            using var connection = factory.CreateConnection();

            var channel = connection.CreateModel();

            //var randomQueueName =channel.QueueDeclare().QueueName;
            //channel.QueueBind(randomQueueName, "logs-fanout", "", null);

            // 3. prop true olursa 6 mesaj varsa sublara böler (2 sub varsa her biri 3er tane mesaj alır) false olursa belirtilen mesaj sayısı kadar (2.propta belirtiyoruz) her sub'a gönderir.   
            channel.BasicQos(0, 1, false);
            // bu satır kalırsa publisherda kuyruk olsa da olmasada hata almaz kendi oluşturur 
            // iki taraftada aynı parametreleri vermeliyiz
            //channel.QueueDeclare("hello-queue", true, false, false);
            var consumer = new EventingBasicConsumer(channel);
            // 2. prop true olursa kuyuruktan direkt siler false olursa biz sonradan silme emri veririz
            //channel.BasicConsume("hello-queue", false, consumer);

            Console.WriteLine("Console Dinleniyor...");

            //var routeKey = "*.Error.*";//ortasındaki ifade error olan 
            //var routeKey = "*.*.Warning";//sonundaki ifade warning olan 
            //var routeKey = "Info.#";//Başındaki ifade Info gerisi önemsiz olan 

            //var queueName = "direct-queue-Critical";
            var queueName = channel.QueueDeclare().QueueName;

            Dictionary<string, object> headers = new Dictionary<string, object>();
            headers.Add("format", "pdf");
            headers.Add("shape", "a4");
            headers.Add("x-match", "all");

            //channel.QueueBind(queueName, "logs-topic", routeKey);
            channel.QueueBind(queueName, "header-exchange", string.Empty, headers);

            //channel.BasicConsume(randomQueueName, false, consumer);

            channel.BasicConsume(queueName, false, consumer);
            consumer.Received += (object sender, BasicDeliverEventArgs e) =>
              {
                  var message = Encoding.UTF8.GetString(e.Body.ToArray());

                  Product product = JsonSerializer.Deserialize<Product>(message);

                  Thread.Sleep(1000);
                  //Console.WriteLine("Gelen Mesaj: " + message);
                  Console.WriteLine($"Gelen Mesaj: + {product.Id }-{ product.Name}-{product.Price}-{product.Stock}");
                  //File.AppendAllText("log-critical.txt", message + "\n");

                  channel.BasicAck(e.DeliveryTag, false);
              };


            Console.ReadLine();
        }
    }
}
