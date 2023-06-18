using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyRabbitMQLib
{
    public static class MyRabbiteMQExtensions
    {
        /// <summary>
        /// 初始化消息队列，并添加Publisher到IoC容器
        /// </summary>
        /// <remarks>从Configuration读取"MyRabbbitMQOptions配置项"</remarks>
        public static IServiceCollection AddMyRabbitMQ(this IServiceCollection services, IConfiguration configuration)
        {
            #region 配置项
            // 从Configuration读取"MyRabbbitMQOptions配置项
            var optionSection = configuration.GetSection("MyRabbitMQOptions");

            // 这个myOptions是当前方法使用
            MyRabbitMQOptions myOptions = new();
            optionSection.Bind(myOptions);

            // 加了这行，才可以注入IOptions<MyRabbitMQOptions>或者IOptionsMonitor<MyRabbitMQOptions>
            services.Configure<MyRabbitMQOptions>(optionSection);
            #endregion

            // 加了这行，才可以注入任意类型参数的 IMyPublisher<> 使用
            services.AddTransient(typeof(IMyPublisher<>), typeof(MyPublisher<>));

            // 创建一个工厂对象，并配置单例注入
            services.AddSingleton(new ConnectionFactory
            {
                UserName = myOptions.UserName,
                Password = myOptions.Password,
                HostName = myOptions.Host,
                Port = myOptions.Port
            });

            return services;
        }

        /// <summary>
        /// IServiceCollection的拓展方法，用于发现自定义的EventHandler并添加到服务容器
        /// </summary> 
        /// <param name="types">包含了自定义Handler的类集合，可以使用assembly.GetTypes()</param> 
        /// <remarks>遍历所有types，将继承自IMyEventHandler的类注册到容器</remarks>
        public static IServiceCollection AddMyRabbitMQEventHandlers(this IServiceCollection services, Type[] types)
        {
            var baseType = typeof(IMyEventHandler);

            foreach (var type in types)
            {
                // baseType可以放type，并且type不是baseType
                if (baseType.IsAssignableFrom(type) && baseType != type)
                {
                    // 瞬态注入配置
                    services.AddTransient(typeof(IMyEventHandler), type);
                }
            }

            return services;
        }

        /// <summary>
        /// 给app拓展方法
        /// </summary>
        /// <remarks>
        /// 在IoC容器里获取到所有继承自IMyEvetnHandler的实现类，并开启消费者
        /// </remarks>
        public static IApplicationBuilder UseMyEventHandler(this IApplicationBuilder app)
        {
            var handlers = app.ApplicationServices.GetServices(typeof(IMyEventHandler));
            var factory = app.ApplicationServices.GetService<ConnectionFactory>();

            // 遍历调用自定义的Begin方法
            foreach (var h in handlers)
            {
                var handler = h as IMyEventHandler;
                handler?.Begin(factory.CreateConnection());
            }

            return app;
        }
    }
}
