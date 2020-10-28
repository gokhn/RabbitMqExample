using Common;
using Common.MassTransit;
using Common.MassTransit.Message;
using MassTransit;
using Models;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RabbitMqProducer
{
    class Program
    {
        static void Main(string[] args)
        {
            MassTransitPublishMethod();

            //BasicQueuePublisher();
            // FanoutExchangePublisher();

            //var lsInvoice = new InvoiceItem { InvoiceId = string.Empty, Values = new Dictionary<string, object>() };

            //lsInvoice.Values.Add("INVOICE.APPL_DATEXT_1014.PLAKA_DETAYLAR", "deneme");
            //lsInvoice.Values.Add("INVOICE.GGG.GUNGOR", "gungor");

            //if (lsInvoice.Values != null && lsInvoice.Values.Any(v => v.Key != null && v.Key.IndexOf(".APPL_DATEXT_1014", StringComparison.Ordinal) > -1))
            //{
            //    var defValues = lsInvoice.Values.Where(v => v.Key != null && v.Key.IndexOf(".APPL_DATEXT_1014", StringComparison.Ordinal) > -1);
            // //   ILines defLines = lObject.DataFields.FieldByName("APPL_DATEXT_1014").Lines;
            //  //  defLines.AppendLine();
            //    foreach (var defValue in defValues)
            //    {

            //        string key = defValue.Key.Split('.')[defValue.Key.Split('.').Length - 1];

            //        object value = defValue.Value;
            //        if (defValue.Value is long)
            //        {
            //            value = Convert.ToDouble(defValue.Value);
            //        }
            //        if (defValue.Value is DateTime)
            //        {
            //            value = Convert.ToDateTime(defValue.Value).ToLocalTime();
            //        }
            //      //  defLines[0].FieldByName(key).Value = value;
            //    }
            //}

        }


        public static void MassTransitPublishMethod()
        {
            var busControl =  BusControl.InitializeBus();
            busControl.Start();
            Console.WriteLine("Started publishing.");

            var message = new NewsletterMailMessage
            {
                MailSubject = "MassTransit Subject",
                Body = "MassTransit Bosdy",
                PublishedTime = DateTime.Now
            };

            busControl.Publish(message);
            Console.ReadLine();
        }

        public static void BasicQueuePublisher()
        {
            Console.WriteLine("RabbitMq First Example");

            var order = new Order { Code = "Telephone", Name = "IPhone", OrderDate = DateTime.Now, CreateUserName = "gokhngungor" };

            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (IConnection connection = factory.CreateConnection())
            using (IModel channel = connection.CreateModel())
            {
                /*
                queue : Oluşturulacak kuyruğun adını belirliyoruz.
                durable : Normal şartlarda kuyruktaki mesajların hepsi bellek üzerinde dizilirler.Hal böyleyken RabbitMQ sunucuları bir sebepten dolayı restart atarlarsa tüm veriler kaybolabilir.durable parametresine true değerini verirsek eğer verilerimiz güvenli bir şekilde sağlamlaştırılacak yani fiziksel hale getirilecektir.
                exclusive : Oluşturulacak bu kuyruğa birden fazla kanalın bağlanıp, bağlanmayacağını belirtir.
                autoDelete : True değerine karşılık tüm mesajlar bitince kuyruğu otomatik imha eder.
                 */
                channel.QueueDeclare(queue: "GokhanG",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                string message = JsonConvert.SerializeObject(order);
                var body = Encoding.UTF8.GetBytes(message);


                /*
                 exchange : Eğer exchange kullanmıyorsanız boş bırakınız. Böylece default exchange devreye girecek ve kullanılmış olacaktır.
                 routingKey : Eğer ki default exchange kullanıyorsanız routingKey olarak oluşturduğunuz kuyruğa verdiğiniz ismin birebir aynısını veriniz.
                 body : Gönderilecek mesajın ta kendisidir.
                 */

                channel.BasicPublish(exchange: "",
                                     routingKey: "GokhanG",
                                     basicProperties: null,
                                     body: body);
                Console.WriteLine($"Gönderilen kişi: {order.CreateUserName}");
            }

            Console.WriteLine(" İlgili kişi gönderildi...");
        }

        public  static void FanoutExchangePublisher()
        {
           
            var rabbitMQService = new RabbitMQService();

            using (var connection = rabbitMQService.GetRabbitMQConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare("order.exchange", ExchangeType.Fanout, true, false, null);

                    channel.QueueDeclare("order.billing", true, false, false, null);
                   
                    channel.QueueBind("order.billing", "order.exchange", "");
                    
                    var publicationAddress = new PublicationAddress(ExchangeType.Fanout, "order.exchange", "");

                    channel.BasicPublish(publicationAddress, null,Encoding.UTF8.GetBytes("Fanout Publish"));
                }
            }

            Console.WriteLine("Fanout Publish İşlemi Gerçekleştirildi");
            Console.ReadLine();
        }
    }

    public class InvoiceItem
    {
        public string InvoiceId { get; set; }


        public Dictionary<string, object> Values { get; set; }
    }


}
