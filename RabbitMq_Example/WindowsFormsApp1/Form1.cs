using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ConnectionFactory factory = new ConnectionFactory();
           
            using (IConnection connection = factory.CreateConnection())
            using (IModel channel = connection.CreateModel())
            {
                channel.ExchangeDeclare("iskuyrugu", type: ExchangeType.Fanout);
                for (int i = 1; i <= 100; i++)
                {
                    byte[] bytemessage = Encoding.UTF8.GetBytes($"is - {i}");

                    IBasicProperties properties = channel.CreateBasicProperties();
                    properties.Persistent = true;

                    channel.BasicPublish(exchange: "iskuyrugu", routingKey: "", basicProperties: properties, body: bytemessage);
                }
            }
        }
    }
}
