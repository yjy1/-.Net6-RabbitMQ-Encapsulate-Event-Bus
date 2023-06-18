using System;
using System.Collections.Generic;
using System.Text;

namespace MyRabbitMQLib
{
    public class MyRabbitMQOptions
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string ExchangeName { get; set; } = "";
    }
}
