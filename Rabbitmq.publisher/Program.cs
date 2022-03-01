﻿using RabbitMQ.Client;
using System;
using System.Linq;
using System.Text;

namespace Rabbitmq.publisher
{
    public enum LogNames
    {
        Critical=1,
        Error=2,
        Warning=3,
        Information=4
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqps://rphunrbp:xUarKgCPyfTdAuOGEmnK-UK6_cR9LIpo@baboon.rmq.cloudamqp.com/rphunrbp");

            using var connection = factory.CreateConnection();

            var channel = connection.CreateModel();

            channel.ExchangeDeclare("logs-direct", durable: true, type: ExchangeType.Direct);

            Enum.GetNames(typeof(LogNames)).ToList().ForEach(x =>
            {
                var routeKey = $"route-{x}";
                var queueName=$"direct-queue-{x}";
                channel.QueueDeclare(queueName, true, false, false);

                channel.QueueBind(queueName, "logs-direct", routeKey,null);
            });

            Enumerable.Range(1, 50).ToList().ForEach(x => 
            {
                LogNames log = (LogNames)new Random().Next(1, 5);

                string message = $"log-type: {log}";

                var messageBody = Encoding.UTF8.GetBytes(message);

                var routeKey = $"route-{log}";

                channel.BasicPublish("logs-direct", routeKey, null, messageBody);

                Console.WriteLine($"log sent : {message}");
            });

            Console.ReadLine();
        }
    }
}
