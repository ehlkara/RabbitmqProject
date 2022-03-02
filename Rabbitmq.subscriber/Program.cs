using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;

namespace Rabbitmq.subscriber
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqps://rphunrbp:xUarKgCPyfTdAuOGEmnK-UK6_cR9LIpo@baboon.rmq.cloudamqp.com/rphunrbp");

            using var connection = factory.CreateConnection();

            var channel = connection.CreateModel();
            channel.ExchangeDeclare("header-exchange", durable: true, type: ExchangeType.Headers);

            //channel.QueueDeclare("hello-queue", true, false, false);

            //channel.ExchangeDeclare("logs-fanout", durable: true, type: ExchangeType.Fanout);

            //var randomQueueName = "log-database-save-queue"; //channel.QueueDeclare().QueueName;

            //channel.QueueDeclare(randomQueueName, true, false, false);

            //var randomQueueName = channel.QueueDeclare().QueueName;

            //channel.QueueBind(randomQueueName, "logs-fanout", "", null);

            channel.BasicQos(0, 1, false);

            var consumer = new EventingBasicConsumer(channel);

            var queueName = channel.QueueDeclare().QueueName;

            //var routeKey = "Info.#";

            Dictionary<string,object> headers = new Dictionary<string, object>();

            headers.Add("format", "pdf");
            headers.Add("shape", "a4");
            headers.Add("x-match", "all");

            //channel.QueueBind(queueName, "logs-topic", routeKey);
            channel.QueueBind(queueName, "header-exchange", String.Empty, headers);

            channel.BasicConsume(queueName,false,consumer);

            Console.WriteLine("Logs listening...");

            consumer.Received+= (object sender, BasicDeliverEventArgs e) => 
            {
                var message = Encoding.UTF8.GetString(e.Body.ToArray());

                Product product = JsonSerializer.Deserialize<Product>(message);

                Thread.Sleep(1500);
                Console.WriteLine($"Incoming message: {product.Id }-{product.Name}-{product.Price}-{product.Stock}");

                //File.AppendAllText("log-critical.txt", message + "\n");

                channel.BasicAck(e.DeliveryTag, false);
            };



            Console.ReadLine();
        }
    }
}
