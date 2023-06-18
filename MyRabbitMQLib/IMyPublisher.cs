﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyRabbitMQLib
{
    /// <summary>
    /// 用于注入使用
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IMyPublisher<T> where T : class
    {
        Task PublishAsync(T data, Encoding encoding = null);
    }
}
