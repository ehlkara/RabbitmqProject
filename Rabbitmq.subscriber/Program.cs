using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
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

            //channel.QueueDeclare("hello-queue", true, false, false);

            channel.BasicQos(0, 1, false);

            var consumer = new EventingBasicConsumer(channel);

            channel.BasicConsume("hello-queue",false,consumer);

            consumer.Received+= (object sender, BasicDeliverEventArgs e) => 
            {
                var message = Encoding.UTF8.GetString(e.Body.ToArray());

                Thread.Sleep(1500);
                Console.WriteLine("Incoming message:" + message);

                channel.BasicAck(e.DeliveryTag, false);
            };



            Console.ReadLine();
        }
    }
}
