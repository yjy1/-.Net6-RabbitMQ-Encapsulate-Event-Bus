using Microsoft.AspNetCore.Mvc;
using MyRabbitMQLib;

namespace WebApplication1.Controllers
{
    [Route("api")]
    [ApiController]
    public class TestController : ControllerBase
    {
        public IMyPublisher<PerryTest> TestPublisher { get; }

        public TestController(IMyPublisher<PerryTest> testPublisher)
        {
            TestPublisher = testPublisher;
        }
        [HttpGet("test")]
        public async Task<string> TestAsync()
        {
            var data = new PerryTest()
            {
                Id = Guid.NewGuid(),
                Name = "AAA",
                Count = 123,
                Remark = "哈哈哈"
            };

            await TestPublisher.PublishAsync(data);

            return "发送了一个消息";
        }
    }
}
