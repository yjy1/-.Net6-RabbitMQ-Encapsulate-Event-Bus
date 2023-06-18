using Newtonsoft.Json;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MyRabbitMQLib
{
    //public abstract class MyEventHandler<T> : IMyEventHandler<T> where T : class
    //{
    //    private IModel _channel;
    //    private string _queueName;
    //    private EventingBasicConsumer _consumer;
    //    public MyEventHandlerOptions Options = new()
    //    {
    //        DisableDeserializeObject = false
    //    };

    //    public void Begin(IConnection connection)
    //    {
    //        var type = typeof(T);
    //        // 获取类上的QueueNameAttribute特性，如果不存在则使用类的完整名
    //        var attr = type.GetCustomAttribute<QueueNameAttribute>();
    //        _queueName = string.IsNullOrWhiteSpace(attr?.QueueName) ? type.FullName : attr.QueueName;

    //        //创建通道
    //        _channel = connection.CreateModel();

    //        _consumer = new EventingBasicConsumer(_channel);
    //        _consumer.Received += MyReceivedHandler;
    //        //消费者
    //        _channel.BasicConsume(_queueName, false, _consumer);
    //    }

    //    // 收到消息后
    //    private void MyReceivedHandler(object sender, BasicDeliverEventArgs e)
    //    {
    //        try
    //        {
    //            // 反序列化为对象
    //            var message = Options.Encoding.GetString(e.Body);
    //            T data = null;
    //            // 如果未配置禁用则不解析，后面抽象方法的data参数会始终为空
    //            if (!Options.DisableDeserializeObject)
    //            {
    //                data = JsonConvert.DeserializeObject<T>(message);
    //            }

    //            OnReceivedAsync(data, message).Wait();

    //            // 确认该消息已被消费
    //            _channel?.BasicAck(e.DeliveryTag, false);
    //        }
    //        catch (Exception ex)
    //        {
    //            OnConsumerException(ex);
    //        }
    //    }

    //    /// <summary>
    //    /// 收到消息 
    //    /// </summary>
    //    /// <param name="data">解析后的对象</param>
    //    /// <param name="message">消息原文</param> 
    //    /// <remarks>Options.DisableDeserializeObject为true时，data始终为null</remarks>
    //    public abstract Task OnReceivedAsync(T data, string message);

    //    /// <summary>
    //    /// 异常
    //    /// </summary>
    //    /// <param name="ex">派生类不重写的话，异常被隐藏</param>
    //    public virtual void OnConsumerException(Exception ex)
    //    {

    //    }
    //}
    public abstract class MyEventHandler<T> : IMyEventHandler<T> where T : class
    {
        private IModel _channel;
        private string _queueName;
        private EventingBasicConsumer _consumer;
        public MyEventHandlerOptions Options = new()
        {
            DisableDeserializeObject = false
        };

        public void Begin(IConnection connection)
        {
            var type = typeof(T);
            // 获取类上的QueueNameAttribute特性，如果不存在则使用类的完整名
            var attr = type.GetCustomAttribute<QueueNameAttribute>();
            _queueName = string.IsNullOrWhiteSpace(attr?.QueueName) ? type.FullName : attr.QueueName;

            //创建通道
            _channel = connection.CreateModel();

            _consumer = new EventingBasicConsumer(_channel);
            _consumer.Received += MyReceivedHandler;
            //消费者
            _channel.BasicConsume(_queueName, false, _consumer);
        }

        // 收到消息后
        private void MyReceivedHandler(object sender, BasicDeliverEventArgs e)
        {
            try
            {
                // 如果未配置禁用则不解析，后面抽象方法的data参数会始终为空
                if (!Options.DisableDeserializeObject)
                {
                    T data = null;
                    // 反序列化为对象
                    var message = Options.Encoding.GetString(e.Body);
                    data = JsonConvert.DeserializeObject<T>(message);
                    OnReceivedAsync(data, message).Wait();

                    // 确认该消息已被消费
                    _channel?.BasicAck(e.DeliveryTag, false);
                }
            }
            catch (Exception ex)
            {
                OnConsumerException(ex);
            }
        }

        /// <summary>
        /// 收到消息 
        /// </summary>
        /// <param name="data">解析后的对象</param>
        /// <param name="message">消息原文</param> 
        /// <remarks>Options.DisableDeserializeObject为true时，data始终为null</remarks>
        public abstract Task OnReceivedAsync(T data, string message);

        /// <summary>
        /// 异常
        /// </summary>
        /// <param name="ex">派生类不重写的话，异常被隐藏</param>
        public virtual void OnConsumerException(Exception ex)
        {

        }

    }
}
