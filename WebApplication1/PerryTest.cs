﻿using MyRabbitMQLib;

namespace WebApplication1
{
    [QueueName("perry.test")]
    public class PerryTest
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public int Count { get; set; }
        public string? Remark { get; set; }
    }
}
