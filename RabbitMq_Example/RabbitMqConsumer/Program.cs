using Common;
using Common.MassTransit.Message;
using MassTransit;
using Models;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;

namespace RabbitMqConsumer
{
    
    class Program
    {
        static string Res;
        static void Main(string[] args)
        {
            MassTransitReceiveMethod();
            //BasicQueueConsumer();
            // FanoutExchangeConsumer(args);
        }

        public static void MassTransitReceiveMethod()
        {
            var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                //var host = cfg.Host(new Uri("rabbitmq://localhost/"), h =>
                // {
                //     h.Username("guest");
                //     h.Password("guest");
                // });

                // cfg.ReceiveEndpoint(host, "DailyNewsletterMail", e =>
                // e.Consumer<NewsletterMailMessageConsumer>());

                cfg.ReceiveEndpoint("event-listener", e =>
                {
                    e.Consumer<NewsletterMailMessageConsumer>();
                });

            });

            busControl.Start();
            Console.WriteLine("Started consuming.");
            Console.ReadLine();
        }
        public static void BasicQueueConsumer()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (IConnection connection = factory.CreateConnection())
            using (IModel channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "GokhanG",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                /*
                   queue : Mesajların alınacağı kuyruk adı.
                   autoAck : Kuruktan alınan mesajın silinip silinmemesini sağlıyor. Bazen kuyruktan alınan mesaj işlenirken beklenmeyen hatalarla karşılaşılabiliyor. O yüzden mesajı başarılı bir şekilde işlemeksizin kuyruktan silinmesini pek önermeyiz.
                   consumer : Tüketici.
                */

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    Order order = JsonConvert.DeserializeObject<Order>(message);
                    Console.WriteLine($" Kodu: {order.Code} Name:{order.Name} [{order.CreateUserName}]");
                };
                channel.BasicConsume(queue: "GokhanG",
                                     autoAck: true,
                                     consumer: consumer);

                Console.WriteLine("Teşekkürler :)");
                Console.ReadLine();
            }
        }

        public static void FanoutExchangeConsumer(string[] args)
        {

            var rabbitMQService = new RabbitMQService();

            using (var connection = rabbitMQService.GetRabbitMQConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    var consumer = new EventingBasicConsumer(channel);

                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body.ToArray());

                        Console.WriteLine("order.billing üzerinden mesaj alındı: {0}", message);
                    };

                    channel.BasicConsume("order.billing", true, consumer);
                    Console.ReadLine();
                }
            }

        }
    }
}
