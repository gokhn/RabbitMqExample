
using System;
using System.Collections.Generic;
using System.Text;
using MassTransit;


namespace Common.MassTransit
{
    public static class BusControl
    {
        public static IBusControl InitializeBus()
        {

          return   Bus.Factory.CreateUsingRabbitMq(cfg =>
                        {
                            cfg.Host(new Uri("rabbitmq://localhost/"), h =>
                            {
                                h.Username("guest");
                                h.Password("guest");
                            });
                        });
        }
    }
}
