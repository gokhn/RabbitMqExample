using MassTransit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Common.MassTransit.Message
{
    public class NewsletterMailMessageConsumer : IConsumer<NewsletterMailMessage>
    {
        public Task Consume(ConsumeContext<NewsletterMailMessage> context)
        {
            var message = context.Message;
            message.ConsumedTime = DateTime.Now;

            Console.WriteLine("Message consumed !\nSubject : " + message.MailSubject + "\nConsumed at: " + message.ConsumedTime + "\nQueueId : " + message.QueueId);

            //todo send mail impr.

            return context.ConsumeCompleted;
        }
        
    }
}
