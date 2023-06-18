using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MyRabbitMQLib
{
    public class MyPublisher<T> : IMyPublisher<T>, IDisposable where T : class
    {
        private readonly MyRabbitMQOptions _myOptions;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly string _queueName;


        /// <summary>
        /// 非注入时使用此构造方法
        /// </summary>
        public MyPublisher(IConnection connection)
        {
            _connection = connection;
        }

        /// <summary>
        /// 依赖注入自动走这个构造方法
        /// </summary>
        /// <param name="optionsMonitor"></param>
        /// <param name="factory"></param>
        public MyPublisher(IOptionsMonitor<MyRabbitMQOptions> optionsMonitor, ConnectionFactory factory)
        {
            _myOptions = optionsMonitor.CurrentValue;
            _connection = factory.CreateConnection();

            // 创建通道
            _channel = _connection.CreateModel();

            // 声明一个Exchange
            _channel.ExchangeDeclare(_myOptions.ExchangeName, ExchangeType.Direct, false, false, null);


            var type = typeof(T);
            // 获取类上的QueueNameAttribute特性，如果不存在则使用类的完整名
            var attr = type.GetCustomAttribute<QueueNameAttribute>();
            _queueName = string.IsNullOrWhiteSpace(attr?.QueueName) ? type.FullName : attr.QueueName;

            // 声明一个队列 
            _channel.QueueDeclare(_queueName, false, false, false, null);

            //将队列绑定到交换机
            _channel.QueueBind(_queueName, _myOptions.ExchangeName, _queueName, null);
        }

        /// <summary>
        /// 发布消息
        /// </summary>
        public Task PublishAsync(T data, Encoding encoding = null)
        {
            // 对象转 object[] 发送
            var msg = JsonConvert.SerializeObject(data);
            byte[] bytes = (encoding ?? Encoding.UTF8).GetBytes(msg);
            _channel.BasicPublish(_myOptions.ExchangeName, _queueName, null, bytes);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            // 结束
            _channel.Close();
            _connection.Close();
        }
    }
}
