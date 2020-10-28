using System;
using System.Collections.Generic;
using System.Text;

namespace Common.MassTransit.Message
{
   public class NewsletterMailMessage
    {
        public string MailSubject  { get; set; }

        public string Body { get; set; }

        public DateTime PublishedTime { get; set; }

        public string QueueId { get { return Guid.NewGuid().ToString(); } }

        public DateTime ConsumedTime { get; internal set; }
    }
}
