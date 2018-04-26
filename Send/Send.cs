using System;
using RabbitMQ.Client;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using Model;
using Newtonsoft.Json;

namespace Send
{
    //Obtém dados do site ethereumprice.org e envia à fila do RabbitMq
    class Program
    {
        static void Main(string[] args)
        {
            while(true)
            {
                var t = new Task(ObterDadosPaginaAsync);
                t.Start();

                //25 seg de intervalo entre buscas
                Thread.Sleep(25000);
            }
        }

        private static void EnviarMensagem(string message)
        {
            //encontra a tag html em que o dado se encontra
            var htmlTag = "<span class=\"update-price\">";

            var inicio = message.IndexOf(htmlTag) + htmlTag.Length;

            var msg = message.Substring(inicio, 100);

            var fim = msg.IndexOf("</span>");

            msg = msg.Substring(0, fim);

            //cria um Json com os dados
            msg = FormatarMensagemJson(
                new Message() {
                    Currency = "Ethereum",
                    CurrentValue = msg
                }
            );

            Program.EnviarMensagemParaFilaMq(msg);
        }

        private static void EnviarMensagemParaFilaMq(string message)
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

                Console.WriteLine(" [x] Sent {0}", message);
            }
            Console.ReadLine();
        }

        private static async void ObterDadosPaginaAsync()
        {
            //página que exibe o preço atual da criptomoeda da plataforma ethereum
            var page = "https://ethereumprice.org/";

            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync(page))
            using (HttpContent content = response.Content)
            {
                var result = await content.ReadAsStringAsync();

                EnviarMensagem(result);
            }
        }

        //Cria um Json com as informações obtidas
        private static string FormatarMensagemJson(Message message)
        {
            return JsonConvert.SerializeObject(message);
        }
    }
}
