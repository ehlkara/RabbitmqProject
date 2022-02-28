using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

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

            //channel.QueueDeclare("hello-queue", true, false, false);

            var consumer = new EventingBasicConsumer(channel);

            channel.BasicConsume("hello-queue",true,consumer);

            consumer.Received+= (object sender, BasicDeliverEventArgs e) => 
            {
                var message = Encoding.UTF8.GetString(e.Body.ToArray());

                Console.WriteLine("Incoming message:" + message);
            };

            Console.ReadLine();
        }
    }
}
