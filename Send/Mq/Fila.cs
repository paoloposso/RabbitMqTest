using System;
using RabbitMQ.Client;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using Model;
using Newtonsoft.Json;

namespace Mq
{
    public class Fila
    {
        public static void EnviarMensagem(string message)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            
            using(var connection = factory.CreateConnection())
            using(var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "navigationData",
                                    durable: false,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null);

                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                                    routingKey: "navigationData",
                                    basicProperties: null,
                                    body: body);

                Console.WriteLine(" [x] Enviado {0}", message);
            }
            Console.ReadLine();
        }
    }
}