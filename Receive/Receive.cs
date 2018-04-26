using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using Model;
using DAL;

//Lê a fila do RabbitMq e grava os dados em uma base SQL Server
class Receive
{
    public static void Main()
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

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);

                Console.WriteLine(" [x] Received {0}", message);

                try
                {
                    new NavigationDataDao().Insert(new Model.NavigationData(message));
                }
                catch(Exception ex) 
                {
                    Console.WriteLine(ex.Message);
                }
            };

            channel.BasicConsume(queue: "navigationData",
                                 autoAck: true,
                                 consumer: consumer);

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}